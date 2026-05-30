namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public class BruiserCharacterActionResolver : CharacterActionResolver
{
    public BruiserCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        List<Cell> targetCells = [];
        // The bruiser's active ability only targets adjacent opponents
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            if (CharacterCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && 
                adjacentCell.Character is not null && 
                adjacentCell.Character.Color != Character.Color)
            {
                targetCells.Add(adjacentCell);
            }
        }
        return targetCells;
    }

    /// <summary>
    /// Returns the target direction from the bruiser's perspective
    /// </summary>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Direction GetTargetDirection(Cell targetCell)
    {
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            if (CharacterCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && 
                adjacentCell == targetCell)
            {
                return direction;
            }  
        }
        throw new InvalidOperationException("The target cell was not found around the bruiser");
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // The bruiser's active ability pushes its target by one cell
        // He can push in one of "three opposing direction" but we first get the central pushing direction
        Direction centralPushingDirection = GetTargetDirection(targetCell);
        // To find the other "opposing directions" we get the next direction 
        // clockwise and counterclockwise from central one
        Direction[] pushingDirections = [centralPushingDirection.GetNext(false), 
            centralPushingDirection, 
            centralPushingDirection.GetNext(true)];
    
        List<Cell> targetMovementDestCells = [];
        foreach (Direction pushingDirection in pushingDirections)
        {
            // The target can only be pushed in an empty tile
            if (targetCell.AdjacentCells.TryGetValue(pushingDirection, out Cell? adjacentCell) && 
                adjacentCell.Character is null)
            {
                targetMovementDestCells.Add(adjacentCell);
            }
        }
        return targetMovementDestCells;
    }

    protected override List<CharacterAction> GenerateActiveAbilityActions()
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
                // The bruiser takes the place of its target after pushing it
                CharacterActionTarget targetMovement = new CharacterActionTarget(targetCell.Character, targetCell.Pos, targetDestCell.Pos);
                CharacterActionTarget bruiserMovement = new CharacterActionTarget(Character, CharacterCell.Pos, targetCell.Pos);
                CharacterAction action = new CharacterAction(Character, [bruiserMovement, targetMovement], true);
                activeAbilityActions.Add(action);
            }
        }

        return activeAbilityActions;
    }
}