namespace LeadersBoardGame.GameLogic.Entities;

public class Board
{
    public Cell[][] Cells { get; }

    public Board(Cell[][] cells)
    {
        Cells = cells;
    }
}