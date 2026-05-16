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

    public Tile(Tile refTile) : this(new Position(refTile.Pos))
    {
        Piece = refTile.Piece is null ? null : new Piece(refTile.Piece);
    }
}