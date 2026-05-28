using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Factories;

public static class BoardFactory
{
    private const int s_columns_count = 7;

    /// <summary>
    /// Returns a board initialized with the necessary board dimensions
    /// </summary>
    /// <returns></returns>
    public static Board Create()
    {
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
}