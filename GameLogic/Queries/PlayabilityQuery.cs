namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;


public static class PlayabilityQuery
{
    /// <summary>
    /// Returns the current (= last) turn if it has an in progress actions phase
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the last history entry was not a turn with an in progress actions phase
    /// </exception>
    private static Turn GetCurrentTurnWithInProgressActionsPhase(GameHistory history, string errorMsg)
    {
        // To check if an action phase is in progress, we get the last turn and check if
        // the actions phase has started but not ended yet
        if (history.Entries.Count > 0 && history.Entries[^1] is Turn turn &&
            turn.ActionsPhase.StartAction is not null && turn.ActionsPhase.EndAction is null)
        {
            return turn;
        }
        throw new InvalidOperationException(errorMsg);
    }

    public static bool CanAct(Game game, GameHistory history, Character character)
    {
        // We get the current turn, which throws an exception if no actions phase is in progress
        return CanAct(game, history, character, GetCurrentTurnWithInProgressActionsPhase(history, "A character cannot act outside of the actions phase"));
    }

    private static bool CanAct(Game game, GameHistory history, Character character, Turn turn)
    {
        // Characters are only allowed to act during a turn matching their team colors
        if (character.Color != turn.Team)
        {
            return false;
        }

        // Nemesis is never allowed to act. She can only be forced to act
        if (character.CharacterType == CharacterType.Nemesis)
        {
            return false;
        }

        // Since only one action per turn is allowed, we look for an action performed by the character
        foreach (IGameAction gameAction in turn.ActionsPhase.Actions)
        {
            if (gameAction is CharacterAction characterAction &&

                characterAction.SourceCharacter == character)
            {
                return false;
            }
        }
        // If thay have not already acted, it means that they can act
        return true;
    }

    public static bool MustAct( Game game, GameHistory history, Character character)
    {
        // We call the current turn getter to throw an exception if no actions phase is in progress
        GetCurrentTurnWithInProgressActionsPhase(history, "A character cannot be forced to act outside of the actions phase");

        // There are currently no way for a character to be forced to play "during this turn"
        return false;
    }

    public static bool MustActNow(Game game, GameHistory history, Character character)
    {
        // We get the current turn, which throws an exception if no actions phase is in progress
        return MustActNow(game, history, character, GetCurrentTurnWithInProgressActionsPhase(history, "A character cannot be forced to act immediately outside of the actions phase"));
    }

    private static bool MustActNow(Game game, GameHistory history, Character character, Turn turn)
    {
        // The Nemesis can be forced to play immediately after an action moving a leader
        if (character.CharacterType == CharacterType.Nemesis &&
            turn.ActionsPhase.Actions.Count > 0 && 
            turn.ActionsPhase.Actions[^1] is CharacterAction characterAction)
        {
            foreach (CharacterActionTarget actionTarget in characterAction.Targets)
            {
                // If we find a leader movement in the action targets, the Nemesis is forced to play
                if (actionTarget.Character.CharacterType.GetCharacterCard().IsLeader() &&
                    actionTarget.OriginPos is not null && actionTarget.DestPos is not null)
                {
                    return true;
                }
            }
        }

        // By default, no character is forced to play now
        return false;
    }

    public static bool CanUseActiveAbility(Game game, GameHistory history, Character character)
    {
        // We call the current turn getter to throw an exception if no actions phase is in progress
        GetCurrentTurnWithInProgressActionsPhase(history, "A character cannot used its active ability outside of the actions phase");

        // First, the character must have an active ability
        if (!character.CharacterType.GetCharacterCard().GetAbilityTypes().Contains(AbilityType.Active))
        {
            return false;
        }

        // Then, their ability activation must not be prevented by a passive ability :
        // 1. Passive ability - Jailer prevents adjacent opponents active ability use
        Cell characterCell = BoardQuery.GetCharacterCellById(game.Board, character.Id);
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            if (characterCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) &&
                adjacentCell.Character is not null && 
                adjacentCell.Character.CharacterType == CharacterType.Jailer &&
                adjacentCell.Character.Color != character.Color)
            {
                return false;
            }
        }
        
        return true;
    }

    public static List<Cell> GetPlayableCharacters(Game game, GameHistory history)
    {
        List<Cell> playableCharacterCells = [];
        // First we get the current turn, which throws an exception if no actions phase is in progress
        Turn turn = GetCurrentTurnWithInProgressActionsPhase(history, "Characters cannot be playable outside of an actions phase");
        // Then we search for characters able to act during the turn
        List<Cell> characterCells = BoardQuery.FindCellsWithCharacter(game.Board);
        foreach (Cell characterCell in characterCells) {
            // If there is a character forced to play immediately, we return them alone
            if (MustActNow(game, history, characterCell.Character!))
            {
                return [characterCell];
            }
            // If a character is allowed to act, we add them to the list
            if (CanAct(game, history, characterCell.Character!, turn))
            {
                playableCharacterCells.Add(characterCell);
            }
        }
        return playableCharacterCells;
    }
}