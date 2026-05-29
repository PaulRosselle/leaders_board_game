namespace LeadersBoardGame.GameLogic.Factories;

using System;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public static class BoardFactory
{
    private const int s_columns_count = 7;

    /// <summary>
    /// Returns a board initialized with the necessary board dimensions
    /// </summary>
    /// <returns></returns>
    public static Board Create()
    {
        // First we add the cells on the board
        Cell[][] cells = new Cell[s_columns_count][];
        for (int x = 0; x < s_columns_count; x++)
        {
            int rowsCount = GetRowsCount(x);
            cells[x] = new Cell[rowsCount];
            for (int y = 0; y < rowsCount; y++)
            {
                cells[x][y] = new Cell(new Position(x, y));
            }
        }

        // Then we add references to adjacent cells to every cell
        for (int x = 0; x < s_columns_count; x++)
        {
            int rowsCount = GetRowsCount(x);
            cells[x] = new Cell[rowsCount];
            for (int y = 0; y < rowsCount; y++)
            {
                Cell currentCell = cells[x][y];
                foreach (Direction direction in Enum.GetValues<Direction>())
                {
                    Cell? adjacentCell = FindAdjacentCell(cells, x, y, direction);
                    if (adjacentCell is not null)
                    {
                        currentCell.SetAdjacentCell(direction, adjacentCell);
                        adjacentCell.SetAdjacentCell(direction, currentCell);
                    }
                }
            }
        }
        return new Board(cells);
    }

    /// <summary>
    /// Returns the rows count within a given column on the board.
    /// Since a board is hexagonally shaped, the rows count varies between columns.
    /// </summary>
    public static int GetRowsCount(int columnIdx)
    {
        // Going from left to right, the rows count starts at 4 then increase until the central column (idx = 3)
        if (columnIdx < 4)
        {
            return columnIdx + 4;
        }
        // Afterward the rows count decreases to reach 4 for the last column
        return 10 - columnIdx;
    }

    /// <summary>
    /// Returns the adjacent cell in a given direction. If there is none, returns null instead
    /// </summary>
    private static Cell? FindAdjacentCell(Cell[][] cells, int originX, int originY, Direction direction)
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
            int centerColumnIdx = cells.Length / 2;
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
        if (nextXPos < 0 || nextXPos > cells.GetUpperBound(0) ||
                nextYPos < 0 || nextYPos > cells.GetUpperBound(1)) {
            return null;
        }
        return cells[nextXPos][nextYPos];
    }
}