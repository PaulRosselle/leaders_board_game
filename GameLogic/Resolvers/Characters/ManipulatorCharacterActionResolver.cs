namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class ManipulatorCharacterActionResolver : CharacterActionResolver
{
    public ManipulatorCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        List<Cell> targetCells = [];
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
        // The manipulator's active ability moves its target by one tile
        return GetAdjacentEmptyCells(targetCell.Pos, 1);
    }

    private Cell? GetTargetInDirection(Direction direction)
    {
        // The manipulator can target non-adjacent opponents visibles in a straight line
        Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, CharacterCell.Pos, direction);
        Cell? targetCell = BoardQuery.FindFirstCellInDirectionMatchingCharacter(Game.Board, CharacterCell.Pos, direction, Character.Color, null);
        return adjacentCell != targetCell ? targetCell : null;
    }
}