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

    /// <summary>
    /// Must be overridden by every child class to be able to create simulation instance matching the class type of "this"
    /// </summary>
    /// <param name="action">Action to perform by the simulation handler</param>
    /// <param name="simulationBoard">Board on which the simulation will occur</param>
    /// <param name="simulationHistory">History impacted by the simulation</param>
    protected abstract PieceActionHandler CreateSimulationHandler(PieceAction action, Board simulationBoard, PieceActionHistory simulationHistory);

    /// <summary>
    /// Returns true if performing the action would lead to a forbidden state of the game.
    /// The forbidden states are currently : capturing or encircling your own leader
    /// </summary>
    protected bool IsInvalidAction(PieceAction action)
    {
        // The more robust way to check is to simulate the action even though this is a slow process
        Board simulationBoard = new Board(Board);
        // We create a new empty history for the simulation since we don't need the complete one to check the action validity
        PieceActionHistory simulationActionHistory = new PieceActionHistory();
        simulationActionHistory.StartNewTurn();
        PieceActionHandler simulationHandler = CreateSimulationHandler(action, simulationBoard, simulationActionHistory);
        simulationHandler.DoAction();
        // We check if the leader is captured or surrounded after the action on the simulation board
        PlayerColor leaderColor = action.SourcePiece.Color;
        return GameEndConditionChecker.IsLeaderCaptured(leaderColor, simulationBoard) || 
               GameEndConditionChecker.IsLeaderSurrounded(leaderColor, simulationBoard);
    }

    /// <summary>
    /// Returns the destination tiles reachable by the movement behavior of the SourcePiece
    /// </summary>
    protected virtual List<Tile> GetMovementFromBehavior()
    {
        // By default, a piece is able to move to any empty adjacent tile
        Tile pieceTile = Board.GetPieceTileById(SourcePiece.Id);
        List<Tile> movement = new List<Tile>();
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Tile? adjacentTile = Board.FindAdjacentTile(pieceTile, direction);
            if (adjacentTile is not null && adjacentTile.Piece is null)
            {
                movement.Add(adjacentTile);
            }
        }
        return movement;
    }

    /// <summary>
    /// Returns a list containing only the valid destinations from the allMovement parameter
    /// </summary>
    protected List<Tile> FilterOutInvalidMovement(List<Tile> allMovement)
    {
        List<Tile> validMovement = new List<Tile>();
        Position originPos = Board.GetPieceTileById(SourcePiece.Id).Pos;
        foreach (Tile destTile in allMovement)
        {
            if (!IsInvalidAction(PieceAction.BuildMovementAction(SourcePiece, originPos, destTile.Pos)))
            {
                validMovement.Add(destTile);
            }
        }
        return validMovement;
    }

    /// <summary>
    /// Returns the destinations tiles reachable by the SourcePiece when using a Movement action.
    /// This function should only be overridden to change the whole movement logic, to implement
    /// a different piece movement behavior, consider overriding "GetMovementFromBehavior"
    /// </summary>
    public virtual List<Tile> GetMovement()
    {
        return FilterOutInvalidMovement(GetMovementFromBehavior());
    }

    public virtual List<Tile> GetTargets()
    {
        return [];
    }

    public virtual List<Tile> GetTargetMovement(Tile targetTile)
    {
        return [];
    }

    /// <summary>
    /// Contains the logic to be applied on a DoAction call
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Handler has been created without an Action. This indicates a programming error.
    /// </exception>
    protected virtual void DoActionBehavior()
    {
        if (Action is null)
        {
            throw new InvalidOperationException("Cannot do action : Action attribute missing");
        }

        // New, Movement and Ability default behavior can be synthetized with the following
        // algorithm since the Action empty fields define what is to do for those action kinds
        if (Action.Kind != PieceActionKind.Exclusion)
        {
            // First, we remove the pieces from their original position
            if (Action.SourceDestPos is not null && Action.SourceOriginPos is not null)
            {
                Board.GetTile(Action.SourceOriginPos).Piece = null;
            }
            if (Action.TargetPiece is not null && Action.TargetDestPos is not null && Action.TargetOriginPos is not null)
            {
                Board.GetTile(Action.TargetOriginPos).Piece = null;
            }
            // Then we add them to their destination
            if (Action.SourceDestPos is not null)
            {
                // We don't assign directly by reference when dealing with PieceAction since
                // an action must keep trace of pieces state at a certain point
                Board.GetTile(Action.SourceDestPos).Piece = new Piece(Action.SourcePiece);
            }
            
            if (Action.TargetPiece is not null && Action.TargetDestPos is not null)
            {
                // We don't assign directly by reference when dealing with PieceAction since
                // an action must keep trace of pieces state at a certain point
                Board.GetTile(Action.TargetDestPos).Piece = new Piece(Action.TargetPiece);
            }
        }
    }

    /// <summary>
    /// Applies the Action effects on the board and adds it to the history
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Handler has been created without an Action. This indicates a programming error.
    /// </exception>
    public void DoAction()
    {
        if (Action is null)
        {
            throw new InvalidOperationException("Cannot do action : Action attribute missing");
        }
        
        DoActionBehavior();
        // In any case, we add the action to the history
        ActionHistory.AddAction(Action);
    }

    /// <summary>
    /// Contains the logic to be applied on a UndoAction call
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Handler has been created without an Action. This indicates a programming error.
    /// </exception>
    protected virtual void UndoActionBehavior()
    {
        if (Action is null)
        {
            throw new InvalidOperationException("Cannot undo action : Action attribute missing");
        }

        // New, Movement and Ability default behavior can be synthetized with the following
        // algorithm since the Action empty fields define what is to do for those action kinds
        if (Action.Kind != PieceActionKind.Exclusion)
        {
            // First, we remove the pieces from their current position
            if (Action.SourceDestPos is not null)
            {
                Board.GetTile(Action.SourceDestPos).Piece = null;
            }
            if (Action.TargetPiece is not null && Action.TargetDestPos is not null)
            {
                Board.GetTile(Action.TargetDestPos).Piece = null;
            }
            // Then we add them back to their original position
            if (Action.SourceOriginPos is not null)
            {
                // We don't assign directly by reference when dealing with PieceAction since
                // an action must keep trace of pieces state at a certain point
                Board.GetTile(Action.SourceOriginPos).Piece = new Piece(Action.SourcePiece);
            }
            
            if (Action.TargetPiece is not null && Action.TargetOriginPos is not null)
            {
                // We don't assign directly by reference when dealing with PieceAction since
                // an action must keep trace of pieces state at a certain point
                Board.GetTile(Action.TargetOriginPos).Piece = new Piece(Action.TargetPiece);
            }
        }
    }

    /// <summary>
    /// Reverts the Action effects on the board and adds it to the history
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Handler has been created without an Action. This indicates a programming error.
    /// </exception>
    public void UndoAction()
    {
        if (Action is null)
        {
            throw new InvalidOperationException("Cannot undo action : Action attribute missing");
        }

        UndoActionBehavior();
        // In any case, we remove the action from the history
        ActionHistory.RemoveAction(Action);
    }
}