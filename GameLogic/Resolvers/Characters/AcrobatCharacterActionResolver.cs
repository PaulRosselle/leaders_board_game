namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public class AcrobatCharacterActionResolver : CharacterActionResolver
{
    public AcrobatCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        // The acrobat's active ability only targets itself
        return [CharacterCell];
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        HashSet<Cell> targetMovementDestCells = [];
        // The acrobat can jump above any adacent character twice per active ability use
        foreach (Direction firstJumpDirection in Enum.GetValues<Direction>())
        {
            Cell? firstJumpDest = FindJumpDestination(CharacterCell, firstJumpDirection);
            if (firstJumpDest is not null)
            {
                targetMovementDestCells.Add(firstJumpDest);
                foreach (Direction secondJumpDirection in Enum.GetValues<Direction>())
                {
                    // The second jump cannot be in the opposite direction of the
                    // first since it would be the same as not moving
                    if (secondJumpDirection == firstJumpDirection.GetOpposite())
                    {
                        continue;
                    }
                    Cell? secondJumpDest = FindJumpDestination(firstJumpDest, secondJumpDirection);
                    if (secondJumpDest is not null)
                    {
                        targetMovementDestCells.Add(secondJumpDest);
                        
                    }
                }
            }
        }
        return [.. targetMovementDestCells];
    }

    private Cell? FindJumpDestination(Cell originCell, Direction direction)
    {
        // The acrobat can jump above any adjacent character if the next cell in the same direction is empty
        if (originCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && adjacentCell.Character is not null &&
            adjacentCell.AdjacentCells.TryGetValue(direction, out Cell? jumpDestCell) && jumpDestCell.Character is null)
        {
            return jumpDestCell;
        }
        return null;
    }
}