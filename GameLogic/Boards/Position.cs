namespace LeadersBoardGame.GameLogic.Boards;

public class Position
{
    // We use byte for coordinates since there are only 37 different positions on the board
    public byte X { get; init; }
    public byte Y { get; init; }

    public Position(byte x, byte y)
    {
        X = x;
        Y = y;
    }

    public Position(Position refPosition) : this(refPosition.X, refPosition.Y)
    {
    }
}