using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Queries;

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
    /// Returns the adjacent cell in a given direction. If there is none, returns null instead
    /// </summary>
    public static Cell? FindAdjacentCell(Board board, int originX, int originY, Direction direction)
    {
        // Since in most cases only one of the coordinate value changes, we initialize them with the origin value
        int nextXPos = originX;
        int nextYPos = originY;
        if (direction.IsSameColumn()) {
            // If we're looking for a tile on the same column, we apply to y the next value in the given direction
            if (direction.IsTop()) {
                nextYPos--;
            } else {
                nextYPos++;
            }
        } else {
            // Knowing the tiles aren't on the same column at this point, we apply to x the next value in the given direction
            if (direction.IsLeft()) {
                nextXPos--;
            } else {
                nextXPos++;
            }
            // Given that C# integer division truncates the value, "Math.Floor" is unnecessary to get the center tile idx
            int centerColumnIdx = board.Cells.Length / 2;
            // Tiles have up to 6 adjacent tiles, meaning that we can see them as hexagons.
            // Foremost, each column is offset from its neighbors since the board itself is hexagonally shaped.
            // For this reason, we must offset the y value for TopLeft/TopRight directions to take this into account
            bool directionIsTowardExterior = (originX == centerColumnIdx) ||
                    (direction.IsLeft() && originX < centerColumnIdx) ||
                    (!direction.IsLeft() && originX > centerColumnIdx);
            int yOffset = directionIsTowardExterior ? -1 : 0;
            // The BottomLeft/BottomRight tiles are always placed immediately below the TopLeft/TopRight adjacentTiles,
            // meaning we can just add 1 to the offset to get their index on the board
            if (!direction.IsTop()) {
                yOffset++;
            }
            nextYPos += yOffset;
        }
        // If the adjacent tile index is out of bounds, we return null
        if (nextXPos < 0 || nextXPos > board.Cells.GetUpperBound(0) ||
                nextYPos < 0 || nextYPos > board.Cells.GetUpperBound(1)) {
            return null;
        }
        return board.Cells[nextXPos][nextYPos];
    }

    /// <summary>
    /// Return cells with a non-null character matching the given parameters
    /// </summary>
    public static List<Cell> FindTilesWithMatchingPiece(Board board, TeamColor? characterColor, CharacterType? characterType)
    {
        List<Cell> cellsWithMatchingPiece = new List<Cell>();
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                if (cell.Character is not null && 
                    (characterColor is null || cell.Character.Color == characterColor) && 
                    (characterType is null || cell.Character.Type == characterType))
                {
                    cellsWithMatchingPiece.Add(cell);
                }
            }
        }
        return cellsWithMatchingPiece;
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
                    cell.Character.Type.GetCharacterCard().IsLeader())
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
    public static Cell GetCharacterCellById(Board board, int pieceId)
    {
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                if (cell.Character is not null && cell.Character.Id == pieceId)
                {
                    return cell;
                }
            }
        }
        throw new InvalidOperationException($"No piece found on the board with id {pieceId}");
    }
}