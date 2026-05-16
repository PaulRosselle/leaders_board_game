using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Boards;
using LeadersBoardGame.GameLogic.Pieces;

namespace LeadersBoardGame.GameLogic.Handlers.Actions;

public abstract class PieceActionHandler
{
    public Piece SourcePiece { get; }
    public PieceAction? Action { get; }
    protected Board Board { get; }
    protected PieceActionHistory ActionHistory { get; }

    public PieceActionHandler(Piece sourcePiece, Board board, PieceActionHistory actionHistory)
    {
        SourcePiece = sourcePiece;
        Board = board;
        ActionHistory = actionHistory;
        Action = null;
    }

    public PieceActionHandler(PieceAction action, Board board, PieceActionHistory actionHistory) : this(action.SourcePiece, board, actionHistory)
    {
        Action = action;
    }

    public virtual bool CanAct()
    {
        return true;
    }

    public virtual bool MustAct()
    {
        return false;
    }

    public virtual bool CanUseAbility()
    {
        return false;
    }

    public virtual List<Tile> GetMovement()
    {
        return [];
    }

    public virtual List<Tile> GetTargets()
    {
        return [];
    }

    public virtual List<Tile> GetTargetMovement(Tile targetTile)
    {
        return [];
    }

    public virtual void DoAction()
    {
    }

    public virtual void UndoAction()
    {
    }
}