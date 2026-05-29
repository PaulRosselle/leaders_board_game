namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class ClawLauncherCharacterActionResolver : CharacterActionResolver
{
    public ClawLauncherCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        // The claw launcher can target itself using his active ability
        List<Cell> targetCells = [CharacterCell];
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Cell? targetCell = GetTargetInDirection(direction);
            if (targetCell is not null)
            {
                targetCells.Add(targetCell);
            }
        }
        return targetCells;
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {   
        List<Cell> targetMovementDestCells = [];
        // The claw launcher active ability has two modes :
        // 1. Dragging a target to him
        if (targetCell != CharacterCell)
        {
            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (GetTargetInDirection(direction) == targetCell)
                {
                    // We add the adjacent cell in the direction of the target (from the claw launcher's pov)
                    targetMovementDestCells.Add(BoardQuery.FindAdjacentCell(Game.Board, CharacterCell.Pos, direction)!);
                    break;
                }
            }
        }
        // 2. Dragging himself to a character visible in a straight line
        else
        {
            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                Cell? destTargetCell = GetTargetInDirection(direction);
                if (destTargetCell is not null)
                {
                    // We add the adjacent cell in the direction of the claw launcher (from the target's pov)
                    targetMovementDestCells.Add(BoardQuery.FindAdjacentCell(Game.Board, destTargetCell.Pos, direction.GetOpposite())!);
                }
            }
        }

        return targetMovementDestCells;
    }
    
    private Cell? GetTargetInDirection(Direction direction)
    {
        // The claw launcher can target non-adjacent characters visibles in a straight line
        Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, CharacterCell.Pos, direction);
        Cell? targetCell = BoardQuery.FindFirstCellInDirectionMatchingCharacter(Game.Board, CharacterCell.Pos, direction, null, null);
        return adjacentCell != targetCell ? targetCell : null;
    }
}