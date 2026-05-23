namespace LeadersBoardGame.GameLogic.Entities;

public class Board
{
    public Cell[][] Cells { get; }

    public Board(Cell[][] cells)
    {
        Cells = cells;
    }

    public Board(Board refBoard)
    {
        int columnsCount = refBoard.Cells.Length;
        Cells = new Cell[columnsCount][];
        for (byte x = 0; x < columnsCount; x++)
        {
            int rowsCount = refBoard.Cells[x].Length;
            Cells[x] = new Cell[rowsCount];
            for (int y = 0; y < rowsCount; y++)
            {
                Cells[x][y] = new Cell(refBoard.Cells[x][y]);
            }
        }
    }
}