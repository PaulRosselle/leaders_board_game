namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;


public static class PlayabilityQuery
{
    /// <summary>
    /// Returns true if the character is allowed to act during the current actions phase.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no actions phase is in progress.
    /// </exception>
    public static bool CanAct(Game game, GameHistory history, Character character)
    {
        // First, we get the current actions phase and turn team
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        TeamColor? currentEntryTeam = GameHistoryQuery.GetLastEntryTeam(history);
        if (currentPhase is ActionsPhase actionsPhase && currentEntryTeam is TeamColor currentTurnTeam)
        {
            return CanAct(game, history, character, actionsPhase, currentTurnTeam);
        }
        // We throw an exception if no action phase in progress could be found
        throw new InvalidOperationException("A character cannot act outside of the actions phase");
    }

    /// <summary>
    /// Returns true if the character is forced to act during the current actions phase.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no actions phase is in progress.
    /// </exception>
    private static bool CanAct(Game game, GameHistory history, Character character, ActionsPhase actionsPhase, TeamColor turnTeam)
    {
        // Characters are only allowed to act during a turn matching their team colors
        if (character.Team != turnTeam)
        {
            return false;
        }

        // Nemesis is never allowed to act. She can only be forced to act
        if (character.CharacterType == CharacterType.Nemesis)
        {
            return false;
        }

        // Since only one action per turn is allowed, we look for an action performed by the character
        foreach (IGameAction gameAction in actionsPhase.Actions)
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

    /// <summary>
    /// Returns true if the character is forced to act immediately.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no actions phase is in progress.
    /// </exception>
    public static bool MustAct(Game game, GameHistory history, Character character)
    {
        // We check if there is an actions phase in progress
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        if (currentPhase is not ActionsPhase)
        {
            throw new InvalidOperationException("A character cannot be forced to act outside of the actions phase");
        }

        // There are currently no way for a character to be forced to play "during this turn"
        return false;
    }

    public static bool MustActNow(Game game, GameHistory history, Character character)
    {
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        if (currentPhase is ActionsPhase actionsPhase)
        {
            return MustActNow(game, history, character, actionsPhase);
        }
        throw new InvalidOperationException("A character cannot be forced to act immediatelty outside of the actions phase");
    }

    private static bool MustActNow(Game game, GameHistory history, Character character, ActionsPhase actionsPhase)
    {
        // The Nemesis can be forced to play immediately after an action moving a leader
        if (character.CharacterType == CharacterType.Nemesis &&
            actionsPhase.Actions.Count > 0 && 
            actionsPhase.Actions[^1] is CharacterAction characterAction)
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

    /// <summary>
    /// Returns true if the character is allowed to use their active ability.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no actions phase is in progress.
    /// </exception>
    public static bool CanUseActiveAbility(Game game, GameHistory history, Character character)
    {
        // We check if there is an actions phase in progress
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        if (currentPhase is not ActionsPhase)
        {
            throw new InvalidOperationException("A character cannot used its active ability outside of the actions phase");
        }

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
                adjacentCell.Character.Team != character.Team)
            {
                return false;
            }
        }
        
        return true;
    }

    public static List<Position> GetPlayableCharacters(Game game, GameHistory history)
    {
        // First, we get the current actions phase and turn team
        IPhase? currentPhase = GameHistoryQuery.GetCurrentPhase(history);
        TeamColor? currentEntryTeam = GameHistoryQuery.GetLastEntryTeam(history);
        if (currentPhase is not ActionsPhase actionsPhase || currentEntryTeam is not TeamColor currentTurnTeam)
        {
            throw new InvalidOperationException("A character cannot used its active ability outside of the actions phase");
        }
        
        List<Position> playableCharacterCells = [];
        // Then we search for characters able to act during the turn
        List<Cell> characterCells = BoardQuery.FindCellsWithCharacter(game.Board);
        foreach (Cell characterCell in characterCells) {
            // If there is a character forced to play immediately, we return them alone
            if (MustActNow(game, history, characterCell.Character!))
            {
                return [characterCell.Pos];
            }
            // If a character is allowed to act, we add them to the list
            if (CanAct(game, history, characterCell.Character!, actionsPhase, currentTurnTeam))
            {
                playableCharacterCells.Add(characterCell.Pos);
            }
        }
        return playableCharacterCells;
    }
}