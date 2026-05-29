namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public static class BoardQuery
{
    /// <summary>
    /// Returns the board cell in the given position
    /// </summary>
    public static Cell GetCell(Board board, Position pos)
    {
        return board.Cells[pos.X][pos.Y];
    }

    /// <summary>
    /// Return cells with a non-null character matching the given parameters
    /// </summary>
    public static List<Cell> FindCellsWithMatchingCharacter(Board board, TeamColor? characterColor, CharacterType? characterType)
    {
        List<Cell> cellsWithMatchingPiece = new List<Cell>();
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                if (cell.Character is not null && 
                    (characterColor is null || cell.Character.Color == characterColor) && 
                    (characterType is null || cell.Character.CharacterType == characterType))
                {
                    cellsWithMatchingPiece.Add(cell);
                }
            }
        }
        return cellsWithMatchingPiece;
    }

    /// <summary>
    /// Find the first cell containing a character in a given direction and returns it if it matches the parameters
    /// </summary>
    public static Cell? FindFirstCellInDirectionMatchingCharacter(Board board, Position pos, Direction direction,
                                                                  TeamColor? characterColor, CharacterType? characterType)
    {
        // If we cant pursue the search in the given direction, we return null
        if (!GetCell(board, pos).AdjacentCells.TryGetValue(direction, out Cell? adjacentCell))
        {
            return null;
        }
        // If the tile is empty, we search in the next cell
        if (adjacentCell.Character is null)
        {
            return FindFirstCellInDirectionMatchingCharacter(board, adjacentCell.Pos, direction, characterColor, characterType);
        }
        // If the character matches the search options, we return its cell
        if ((characterColor is null || characterColor == adjacentCell.Character.Color) &&
            (characterType is null || characterType == adjacentCell.Character.CharacterType))
        {
            return adjacentCell;
        }
        return null;
    }

    /// <summary>
    /// Returns the first cell with a leader of the expected color. If there is none, returns null instead
    /// </summary>
    public static Cell? FindLeaderCell(Board board, TeamColor leaderColor)
    {
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                if (cell.Character is not null && 
                    cell.Character.Color == leaderColor &&
                    cell.Character.CharacterType.GetCharacterCard().IsLeader())
                {
                    return cell;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the first cell containing a leader of the expected color.
    /// </summary>
    /// <param name="leaderColor"></param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no leader could be found.
    /// </exception>
    public static Cell GetLeaderCell(Board board, TeamColor leaderColor)
    {
        Cell? leaderCell = FindLeaderCell(board, leaderColor);
        if (leaderCell is null)
        {
            throw new InvalidOperationException($"No leader found for player {leaderColor}");
        }
        return leaderCell;
    }

    /// <summary>
    /// Returns the first cell containing a character with a matching Id.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the character is not on the board during the call. This indicates a programming error.
    /// </exception>
    public static Cell GetCharacterCellById(Board board, Guid characterId)
    {
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                if (cell.Character is not null && cell.Character.Id == characterId)
                {
                    return cell;
                }
            }
        }
        throw new InvalidOperationException($"No piece found on the board with id {characterId}");
    }
}