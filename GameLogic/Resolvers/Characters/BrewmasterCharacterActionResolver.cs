namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class BrewmasterCharacterActionResolver : CharacterActionResolver
{
    public BrewmasterCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        List<Cell> targetCells = [];
        // The brewmaster's active ability only targets adjacent allies
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, CharacterCell, direction);
            if (adjacentCell is not null && adjacentCell.Character is not null && 
                adjacentCell.Character.Color == Character.Color)
            {
                targetCells.Add(adjacentCell);
            }
        }
        return targetCells;
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // The brewmaster's active ability moves its target by one tile
        return GetAdjacentEmptyCells(targetCell.Pos, 1);
    }
}