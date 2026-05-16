namespace LeadersBoardGame.GameLogic.Pieces;

using LeadersBoardGame.GameLogic.Players;

public class Piece
{
    public int Id { get; init; }
    public PlayerColor Color { get; init; }
    public PieceKind Kind { get; set; }

    public Piece(int id, PlayerColor color, PieceKind kind)
    {
        Id = id;
        Color = color;
        Kind = kind;
    }

    public Piece(Piece refPiece) : this(refPiece.Id, refPiece.Color, refPiece.Kind)
    {
    }

    /// <summary>
    /// Compare the piece with another and returns true if they share the same color and kind
    /// </summary>
    public bool IsSimilar(Piece otherPiece)
    {
        return Color == otherPiece.Color && Kind == otherPiece.Kind;
    }
}