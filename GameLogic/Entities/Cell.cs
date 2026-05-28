namespace LeadersBoardGame.GameLogic.Entities;

public class Cell
{
    public Position Pos { get; }
    public Character? Character { get; set; }

    public Cell(Position pos)
    {
        Pos = pos;
    }
}