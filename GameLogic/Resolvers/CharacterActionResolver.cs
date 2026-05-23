
using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Queries;

namespace LeadersBoardGame.GameLogic.Resolvers;

public abstract class CharacterActionResolver
{
    protected Game Game { get; }
    protected Character Character { get; }
    protected Cell CharacterCell { get; }

    public CharacterActionResolver(Game game, Character character)
    {
        Game = game;
        Character = character;
        CharacterCell = BoardQuery.GetCharacterCellById(game.Board, character.Id);
    }

    protected bool IsValidAction(CharacterAction action)
    {
        // TODO
        // 1. call CharacterActionFactory and get the according actionHandler
        // 2. call actionHandler.DoAction
        // 3. test if the leader is captured/surrounded
        // 4. call actionHandler.UndoAction
        throw new NotImplementedException();
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
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, currentCellPos, direction);
            // If we encounter an adjacent empty cell, we try to add it to the the list.
            // We only add it if it is a new one and we only recurse if that's the case
            if (adjacentCell is not null && adjacentCell.Character is null && adjacentEmptyCells.Add(adjacentCell) && currentDistance < maxDistance)
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
        // Passive ability : the vizier allows its leader to move to up to two cells per action
        if (Character.CharacterType.GetCharacterCard().IsLeader() && 
            BoardQuery.FindCellsWithMatchingCharacter(Game.Board, Character.Color, CharacterType.Vizier).Count > 0)
        {
            return 2;
        }
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
        // The most whitespread behavior is for a character to target itself using its ability
        return [CharacterCell];
    }

    /// <summary>
    /// Returns true if a destination is required for a targeted action to be valid
    /// </summary>
    protected virtual bool IsTargetMovementDestinationRequired()
    {
        return true;
    }

    /// <summary>
    /// Returns the destinations tiles reachable by the targetTile when using an Ability action.
    /// This function should only be overridden to change the whole target movement logic, to implement
    /// a different target movement behavior, consider overriding "GetTargetMovementFromBehavior"
    /// </summary>
    protected virtual List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // The most whitespread behavior is for a target to be moved to an empty adjacent cell
        return GetAdjacentEmptyCells(targetCell.Pos, 1);
    }

    /// <summary>
    /// Returns every movement action doable by the character
    /// </summary>
    public List<CharacterAction> GetMovementActions()
    {
        List<CharacterAction> movementActions = [];
        foreach (Cell targetCell in GetActiveAbilityTargets())
        {
            if (IsTargetMovementDestinationRequired())
            {
                foreach (Cell destCell in GetTargetMovementDestinations(targetCell))
                {
                    // TODO
                    // 1. generate the action using CharacterActionFactory
                    // 2. add the action to movementActions
                    throw new NotImplementedException();
                }
            }
            else
            {
                // TODO
                // 1. generate the action using CharacterActionFactory
                // 2. add the action to movementActions
                throw new NotImplementedException();
            }
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
        List<CharacterAction> activeAbilityActions = [];
        foreach (Cell destCell in GetMovementDestinations())
        {
            // TODO
            // 1. generate the action using CharacterActionFactory
            // 2. add the action to activeAbilityActions
        }
        // We filter out every invalid actions
        return GetValidActions(activeAbilityActions);
    }
}