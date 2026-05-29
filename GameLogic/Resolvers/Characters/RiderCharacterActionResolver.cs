namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

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
            if (CharacterCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && adjacentCell.Character is null &&
                adjacentCell.AdjacentCells.TryGetValue(direction, out Cell? targetDestCell) && targetDestCell.Character is null)
            {
                targetMovementDestCells.Add(targetDestCell);
            }
        }
        return targetMovementDestCells;
    }
}