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

    /// <summary>
    /// Returns true when the SourcePiece can act during the turn.
    /// </summary>
    public virtual bool CanAct()
    {
        // The default behavior is to allow a piece if it hasn't already acted this turn.
        // We detect if an action has already been made by checking the current turn history.
        // If there are no turn registered, we can return true since there won't be any action to check.
        // This also avoid an unnecessary a call to "GetLastTurnActions" which would raise an exception
        if (ActionHistory.ActionsPerTurn.Count == 0)
        {
            return true;
        }

        List<PieceAction> lastTurnActions = ActionHistory.GetLastTurnActions();
        foreach (PieceAction action in lastTurnActions)
        {
            // We only compare the Id because some action could have changed other fields in the piece
            if (action.SourcePiece.Id == SourcePiece.Id)
            {
                return false;
            }
        }
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