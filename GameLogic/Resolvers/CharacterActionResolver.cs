
using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Queries;
using LeadersBoardGame.GameLogic.Factories;
using LeadersBoardGame.GameLogic.Handlers;

namespace LeadersBoardGame.GameLogic.Resolvers;

public abstract class CharacterActionResolver
{
    protected Game Game { get; }

    protected GameHistory History { get; }

    protected Character Character { get; }

    protected Cell CharacterCell { get; }

    public CharacterActionResolver(Game game, GameHistory history, Character character)
    {
        Game = game;
        History = history;
        Character = character;
        CharacterCell = BoardQuery.GetCharacterCellById(game.Board, character.Id);
    }

    protected bool IsValidAction(CharacterAction action)
    {
        // To check if an action is valid we apply it to the game's projection
        TeamColor team = Character.Team;
        IActionHandler actionHandler = GameActionHandlerFactory.Create(Game, action);
        actionHandler.DoAction();
        // Then we verify each validation condition (a character action is invalid if it captures or surround its allied leader)
        bool isValid = !GameQuery.IsLeaderCaptured(Game, History, team) && !GameQuery.IsLeaderSurrounded(Game, team);
        // And finally we revert the action's effect on the game's projection
        actionHandler.UndoAction();
        return isValid;
    }

    protected virtual List<CharacterAction> GetValidActions(List<CharacterAction> actions)
    {
        List<CharacterAction> validActions = [];
        foreach (CharacterAction action in actions)
        {
            if (IsValidAction(action))
            {
                validActions.Add(action);
            }
        }
        return validActions;
    }

    /// <summary>
    /// Returns adjacent cells around the originCellPos until maxDistance is reached
    /// </summary>
    protected List<Cell> GetAdjacentEmptyCells(Position originCellPos, int maxDistance)
    {
        HashSet<Cell> adjacentEmptyCells = new HashSet<Cell>();
        // Since the distance to an immediately adjacent cell is 1, we initialize "currentDistance" with this value
        GatherAdjacentEmptyCells(originCellPos, adjacentEmptyCells, maxDistance, 1);
        return [.. adjacentEmptyCells];
    }

    /// <summary>
    /// Fills recursively "adjacentEmptyCells" with the adjacent cells around the originCellPos until maxDistance is reached
    /// </summary>
    private void GatherAdjacentEmptyCells(Position currentCellPos, HashSet<Cell> adjacentEmptyCells, int maxDistance, int currentDistance)
    {
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            // If we encounter an adjacent empty cell, we try to add it to the the list.
            // We only add it if it is a new one and we only recurse if that's the case
            if (BoardQuery.GetCell(Game.Board,currentCellPos).AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && 
                adjacentCell.Character is null && adjacentEmptyCells.Add(adjacentCell) && currentDistance < maxDistance)
            {   
                GatherAdjacentEmptyCells(adjacentCell.Pos, adjacentEmptyCells, maxDistance, currentDistance + 1);
            }
        }
    }

    /// <summary>
    /// Returns the movement max distance for the character based on its ability and external factors on the board
    /// </summary>
    protected virtual int GetMovementMaxDistance()
    {
        // By default, a piece can move to an immediately adjacent cell
        return 1;
    }

    /// <summary>
    /// Returns the destination cells reachable by the character
    /// </summary>
    protected virtual List<Cell> GetMovementDestinations()
    {
        return GetAdjacentEmptyCells(CharacterCell.Pos, GetMovementMaxDistance());
    }

    /// <summary>
    /// Returns the cells targetable by the character's active ability
    /// </summary>
    protected virtual List<Cell> GetActiveAbilityTargets()
    {
        // There is no default target behavior because of the variety of active abilities
        return [];
    }

    /// <summary>
    /// Returns the destinations tiles reachable by the targetTile when using an Ability action.
    /// This function should only be overridden to change the whole target movement logic, to implement
    /// a different target movement behavior, consider overriding "GetTargetMovementFromBehavior"
    /// </summary>
    protected virtual List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // There is no default target movement behavior because of the variety of active abilities
        return [];
    }

    /// <summary>
    /// Generates and return every action using an active ability doable by the character
    /// </summary>
    protected virtual List<CharacterAction> GenerateActiveAbilityActions()
    {
        List<CharacterAction> activeAbilityActions = [];
        // The active ability action generation moves by default the target
        // to a destination since it is the most whitespread behavior
        foreach (Cell targetCell in GetActiveAbilityTargets())
        {
            if (targetCell.Character is null)
            {
                throw new InvalidOperationException("Invalid active ability target : the targeted cell contains no character");
            }
            foreach (Cell targetDestCell in GetTargetMovementDestinations(targetCell))
            {
                CharacterActionTarget actionTarget = new CharacterActionTarget(targetCell.Character, targetCell.Pos, targetDestCell.Pos);
                CharacterAction action = new CharacterAction(Character, [actionTarget], true);
                activeAbilityActions.Add(action);
            }
        }

        return activeAbilityActions;
    }

    /// <summary>
    /// Returns every movement action doable by the character
    /// </summary>
    public List<CharacterAction> GetMovementActions()
    {
        List<CharacterAction> movementActions = [];
        foreach (Cell destCell in GetMovementDestinations())
        {
            // Movement actions only targets the sourceCharacter and require both an originPos and destPos
            CharacterActionTarget actionTarget = new CharacterActionTarget(Character, CharacterCell.Pos, destCell.Pos);
            CharacterAction action = new CharacterAction(Character, [actionTarget], false);
            movementActions.Add(action);
        }
        // We filter out every invalid actions
        return GetValidActions(movementActions);
    }

    /// <summary>
    /// Returns every action using an active ability doable by the character
    /// </summary>
    /// <returns></returns>
    public List<CharacterAction> GetActiveAbilityActions()
    {
        // We filter out every invalid actions
        return GetValidActions(GenerateActiveAbilityActions());
    }
}