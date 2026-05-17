namespace LeadersBoardGame.GameLogic.Actions;

using LeadersBoardGame.GameLogic.Pieces;
using LeadersBoardGame.GameLogic.Boards;

public class PieceAction
{
    public PieceActionKind Kind { get; }
    public Piece SourcePiece { get; }
    public Position? SourceOriginPos { get; }
    public Position? SourceDestPos { get; }
    public Piece? TargetPiece { get; }
    public Position? TargetOriginPos { get; }
    public Position? TargetDestPos { get; }

    /// <summary>
    /// Creates a new PieceAction instance. Every field is affected with a copy of the references given as parameters
    /// </summary>
    /// <param name="kind">Indicates the structure and purpose of the action</param>
    /// <param name="sourcePiece">Piece initiating the action</param>
    /// <param name="sourceOriginPos">Original position of the source piece</param>
    /// <param name="sourceDestPos">Destination of the source piece</param>
    /// <param name="targetPiece">Piece targeted by the action</param>
    /// <param name="targetOriginPos">Original position of the targeted piece</param>
    /// <param name="targetDestPos">Destination of the targeted piece</param>
    private PieceAction(PieceActionKind kind, Piece sourcePiece, Position? sourceOriginPos, Position? sourceDestPos, 
                        Piece? targetPiece, Position? targetOriginPos, Position? targetDestPos)
    {
        Kind = kind;
        SourcePiece = new Piece(sourcePiece);
        SourceOriginPos = sourceOriginPos is null ? null : new Position(sourceOriginPos);
        SourceDestPos = sourceDestPos is null ? null : new Position(sourceDestPos);
        TargetPiece = targetPiece is null ? null : new Piece(targetPiece);
        TargetOriginPos = targetOriginPos is null ? null : new Position(targetOriginPos);
        TargetDestPos = targetDestPos is null ? null : new Position(targetDestPos);
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="refPieceAction">Action to be copied</param>
    public PieceAction(PieceAction refPieceAction) : this(refPieceAction.Kind, 
                                                          refPieceAction.SourcePiece, refPieceAction.SourceOriginPos, refPieceAction.SourceDestPos, 
                                                          refPieceAction.TargetPiece, refPieceAction.TargetOriginPos, refPieceAction.TargetDestPos)
    {
    }

    /// <summary>
    /// Compare the action with another and returns true if all their field's value are identical
    /// </summary>
    public bool IsSimilar(PieceAction otherPieceAction)
    {
        return Kind == otherPieceAction.Kind && 
                SourcePiece == otherPieceAction.SourcePiece &&
                SourceOriginPos == otherPieceAction.SourceOriginPos && 
                SourceDestPos == otherPieceAction.SourceDestPos &&
                TargetPiece == otherPieceAction.TargetPiece &&
                TargetOriginPos == otherPieceAction.TargetOriginPos &&
                TargetDestPos == otherPieceAction.TargetDestPos;
    }

    /// <summary>
    /// Builds a New action instance. Every field is affected with a copy of the references given as parameters
    /// </summary>
    /// <param name="sourcePiece">Piece added to the board</param>
    /// <param name="sourceDestPos">Destination of the source piece on the board</param>
    public static PieceAction BuildNewAction(Piece sourcePiece, Position sourceDestPos)
    {
        return new PieceAction(PieceActionKind.New, sourcePiece, null, sourceDestPos, null, null, null);
    }

    /// <summary>
    /// Builds an Exclusion action instance. Every field is affected with a copy of the references given as parameters
    /// </summary>
    /// <param name="sourcePiece">Piece excluded from the game</param>
    public static PieceAction BuildExclusionAction(Piece sourcePiece)
    {
        return new PieceAction(PieceActionKind.Exclusion, sourcePiece, null, null, null, null, null);
    }

    /// <summary>
    /// Builds a movement action instance. Every field is affected with a copy of the references given as parameters
    /// </summary>
    /// <param name="sourcePiece">Piece moved by the action</param>
    /// <param name="sourceOriginPos">Original position of the source piece</param>
    /// <param name="sourceDestPos">Destination of the source piece</param>
    public static PieceAction BuildMovementAction(Piece sourcePiece, Position sourceOriginPos, Position sourceDestPos)
    {
        return new PieceAction(PieceActionKind.Movement, sourcePiece, sourceOriginPos, sourceDestPos, null, null, null);
    }

    /// <summary>
    /// Builds an ability action instance. Every field is affected with a copy of the references given as parameters
    /// </summary>
    /// <param name="sourcePiece">Piece initiating the action</param>
    /// <param name="sourceOriginPos">Original position of the source piece</param>
    /// <param name="sourceDestPos">Destination of the source piece</param>
    /// <param name="targetPiece">Piece targeted by the action</param>
    /// <param name="targetOriginPos">Original position of the targeted piece</param>
    /// <param name="targetDestPos">Destination of the targeted piece</param>
    public static PieceAction BuildAbilityAction(Piece sourcePiece, Position? sourceOriginPos, Position? sourceDestPos, 
                                                 Piece targetPiece, Position? targetOriginPos, Position? targetDestPos)
    {
        return new PieceAction(PieceActionKind.Ability, sourcePiece, sourceOriginPos, sourceDestPos, targetPiece, targetOriginPos, targetDestPos);
    }
}