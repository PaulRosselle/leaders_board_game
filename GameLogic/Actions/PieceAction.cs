namespace LeadersBoardGame.Actions;

using LeadersBoardGame.Pieces;
using LeadersBoardGame.Boards;

public class PieceAction
{
    public PieceActionKind Kind { get; init; }
    public Piece SourcePiece { get; init; }
    private Position? _sourceOriginPos;
    public Position? SourceOriginPos { get => _sourceOriginPos; set => _sourceOriginPos = value is null ? null : new Position(value); }
    private Position? _sourceDestPos;
    public Position? SourceDestPos { get => _sourceDestPos; set => _sourceDestPos = value is null ? null : new Position(value); }
    private Piece? _targetPiece;
    public Piece? TargetPiece { get => _targetPiece; set => _targetPiece = value is null ? null : new Piece(value); }
    private Position? _targetOriginPos;
    public Position? TargetOriginPos { get => _targetOriginPos; set => _targetOriginPos = value is null ? null : new Position(value); }
    private Position? _targetDestPos;
    public Position? TargetDestPos { get => _targetDestPos; set => _targetDestPos = value is null ? null : new Position(value); }

    public PieceAction(PieceActionKind kind, Piece sourcePiece)
    {
        Kind = kind;
        SourcePiece = sourcePiece;
    }

    public PieceAction(PieceAction refPieceAction) : this(refPieceAction.Kind, new Piece(refPieceAction.SourcePiece))
    {
        _sourceOriginPos = refPieceAction.SourceOriginPos is null ? null : new Position(refPieceAction.SourceOriginPos);
        _sourceDestPos = refPieceAction.SourceDestPos is null ? null : new Position(refPieceAction.SourceDestPos);

        _targetPiece = refPieceAction.TargetPiece is null ? null : new Piece(refPieceAction.TargetPiece);
        TargetOriginPos = refPieceAction.TargetOriginPos is null ? null : new Position(refPieceAction.TargetOriginPos);
        _targetDestPos = refPieceAction.TargetDestPos is null ? null : new Position(refPieceAction.TargetDestPos);
    }

    /// <summary>
    /// Compare the action with another and returns true if all their field's value are identical
    /// </summary>
    public bool IsSimilar(PieceAction otherPieceAction)
    {
        return Kind == otherPieceAction.Kind && 
                SourcePiece == otherPieceAction.TargetPiece &&
                SourceOriginPos == otherPieceAction.SourceOriginPos && 
                SourceDestPos == otherPieceAction.SourceDestPos &&
                TargetPiece == otherPieceAction.TargetPiece &&
                TargetOriginPos == otherPieceAction.TargetOriginPos &&
                TargetDestPos == otherPieceAction.TargetDestPos;
    }
}