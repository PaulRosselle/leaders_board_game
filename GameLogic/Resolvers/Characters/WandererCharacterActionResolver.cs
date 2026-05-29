namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class WandererCharacterActionResolver : CharacterActionResolver
{
    public WandererCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override List<Cell> GetActiveAbilityTargets()
    {
        // The wanderer's active ability only targets itself
        return [CharacterCell];
    }

    protected override List<Cell> GetTargetMovementDestinations(Cell targetCell)
    {
        // The wanderer's active ability allows her to fly to any cell non-adjacent to an opponent
        HashSet<Cell> targetMovementDestCells = [];
        
        // First we add every empty cell on the board
        foreach (Cell[] cells in Game.Board.Cells)
        {
            foreach (Cell cell in cells)
            {
                if (cell.Character is null)
                {
                    targetMovementDestCells.Add(cell);
                }
            }
        }
        // Then we remove every cell adjacent to an opponent
        TeamColor opponentColor = Character.Color.GetOpposite();
        foreach (Cell opponentCell in BoardQuery.FindCellsWithMatchingCharacter(Game.Board, opponentColor, null))
        {
            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                Cell? adjacentCell = BoardQuery.FindAdjacentCell(Game.Board, CharacterCell.Pos, direction);
                if (adjacentCell is not null)
                {
                    targetMovementDestCells.Remove(adjacentCell);
                }
            }
        }
        
        return [.. targetMovementDestCells];
    }
}