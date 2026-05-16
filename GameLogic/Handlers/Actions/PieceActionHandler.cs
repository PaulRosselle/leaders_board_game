namespace LeadersBoardGame.GameLogic.Handlers.Actions;

using System;
using System.Collections.Generic;
using System.Linq;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Boards;
using LeadersBoardGame.GameLogic.Cards;
using LeadersBoardGame.GameLogic.Pieces;
using LeadersBoardGame.GameLogic.Players;

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

    /// <summary>
    /// Returns true when the SourcePiece should be forced to play immediately.
    /// </summary>
    public virtual bool MustAct()
    {
        // Piece action is optional by default
        return false;
    }

    /// <summary>
    /// Returns true if the SourcePiece has an active ability.
    /// </summary>
    private bool HasActiveAbility()
    {
        return SourcePiece.Kind.GetCardKind().GetAbilityKinds().Contains(AbilityKind.Active);
    }

    /// <summary>
    /// Returns true when at least an adjacent piece matching the pieceKind parameter is found.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the SourcePiece is not on the board during the call.
    /// </exception>
    private bool HasAdjacentPiece(PlayerColor pieceColor, PieceKind pieceKind)
    {
        Tile pieceTile = Board.GetPieceTileById(SourcePiece.Id);
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            if (Board.FindAdjacentTile(pieceTile, direction)?.Piece is { } piece && 
                piece.Kind == pieceKind && piece.Color == pieceColor)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true when the SourcePiece is able to use an Active ability.
    /// </summary>
    public virtual bool CanUseAbility()
    {
        // The default behavior takes into account :
        // 1. If the SourcePiece has an Active ability
        // 2. If the ability use is not prevented by a character passive ability

        // Passive Ability : the Jailer prevents its adjacent opponents to use their active ability
        return HasActiveAbility() && !HasAdjacentPiece(SourcePiece.Color.GetOpposite(), PieceKind.Jailer);
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