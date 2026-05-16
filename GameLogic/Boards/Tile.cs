using LeadersBoardGame.Pieces;

namespace LeadersBoardGame.Boards;

public class Tile
{
    public Position Pos { get; init; }
    public Piece? Piece { get; set; }

    public Tile(Position pos)
    {
        Pos = pos;
    }
}