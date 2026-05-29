namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class RiderCharacterActionResolver : CharacterActionResolver
{
    public RiderCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        // The rider's active ability only targets itself
        return [CharacterCell];
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        List<Cell> targetMovementDestCells = [];
        // The rider's active ability allows him to run by 2 cells in a straight line
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, CharacterCell.Pos, direction);
            if (adjacentCell is not null && adjacentCell.Character is null)
            {
                Cell? targetDestCell = BoardQuery.FindAdjacentCell(Game.Board, adjacentCell.Pos, direction);
                if (targetDestCell is not null && targetDestCell.Character is null)
                {
                    targetMovementDestCells.Add(targetDestCell);
                }
            }
        }
        return targetMovementDestCells;
    }
}