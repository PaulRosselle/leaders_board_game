using System.Security.Cryptography.X509Certificates;

namespace LeadersBoardGame.Boards;

public static class BoardConfig
{
    private static byte s_columns_count = 7;

    public static byte GetColumnsCount()
    {
        return s_columns_count;
    }

    /// <summary>
    /// Returns the rows count within a given column on the board.
    /// Since a board is hexagonally shaped, the rows count varies between columns.
    /// </summary>
    public static byte GetRowsCount(int columnIdx)
    {
        // Going from left to right, the rows count starts at 4 then increase until the central column (idx = 3)
        if (columnIdx < 4)
        {
            return (byte)(columnIdx + 4);
        }
        // Afterward the rows count decreases to reach 4 for the last column
        return (byte)(10 - columnIdx);
    }
}