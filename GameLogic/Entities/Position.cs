namespace LeadersBoardGame.GameLogic.Entities;

public class Position
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Position(Position refPosition) : this(refPosition.X, refPosition.Y)
    {
    }
}