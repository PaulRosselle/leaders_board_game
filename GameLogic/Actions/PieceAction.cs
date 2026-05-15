namespace LeadersBoardGame.Actions;

using LeadersBoardGame.Pieces;
using LeadersBoardGame.Boards;

public class PieceAction
{
    public PieceActionKind Kind { get; init; }
    public Piece SourcePiece { get; init; }
    private Position? _sourceOriginPos;
    public Position? SourceOriginPos { get => _sourceOriginPos; set => _sourceOriginPos = value is null ? null : value; }
    private Position? _sourceDestPos;
    public Position? SourceDestPos { get => _sourceDestPos; set => _sourceDestPos = value is null ? null : value; }
    private Piece? _targetPiece;
    public Piece? TargetPiece { get => _targetPiece; set => _targetPiece = value is null ? null : value; }
    private Position? _targetOriginPos;
    public Position? TargetOriginPos { get => _targetOriginPos; set => _targetOriginPos = value is null ? null : value; }
    private Position? _targetDestPos;
    public Position? TargetDestPos { get => _targetDestPos; set => _targetDestPos = value is null ? null : value; }

    public PieceAction(PieceActionKind kind, Piece sourcePiece)
    {
        Kind = kind;
        SourcePiece = sourcePiece;
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