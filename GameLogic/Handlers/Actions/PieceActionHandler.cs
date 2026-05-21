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
    /// Returns adjacent tiles around the originTilePos until maxDistance is reached
    /// </summary>
    protected List<Tile> GetAdjacentEmptyTiles(Position originTilePos, int maxDistance)
    {
        HashSet<Tile> adjacentEmptyTiles = new HashSet<Tile>();
        // Since the distance to an immediately adjacent tile is 1, we initialize "currentDistance" with this value
        GatherAdjacentEmptyTiles(originTilePos, adjacentEmptyTiles, maxDistance, 1);
        return adjacentEmptyTiles.ToList();
    }

    /// <summary>
    /// Fills recursively "adjacentEmptyTiles" with the adjacent tiles around the originTilePos until maxDistance is reached
    /// </summary>
    private void GatherAdjacentEmptyTiles(Position currentTilePos, HashSet<Tile> adjacentEmptyTiles, int maxDistance, int currentDistance)
    {
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Tile? adjacentTile = Board.FindAdjacentTile(currentTilePos, direction);
            // If we encounter an adjacent empty tile, we try to add it to the the list.
            // We only add it if it is a new one and we only recurse if that's the case
            if (adjacentTile is not null && adjacentTile.Piece is null && adjacentEmptyTiles.Add(adjacentTile) && currentDistance < maxDistance)
            {   
                GatherAdjacentEmptyTiles(adjacentTile.Pos, adjacentEmptyTiles, maxDistance, currentDistance + 1);
            }
        }
    }

    /// <summary>
    /// Returns the movement max distance allowed by the SourcePiece movement behavior
    /// </summary>
    protected virtual int GetMovementDistance()
    {
        // Passive ability : the vizier allows its leader to move to up to two tiles per action
        if (SourcePiece.Kind.GetCardKind().IsLeader() && Board.FindTilesWithMatchingPiece(SourcePiece.Color, PieceKind.Vizier).Count > 0)
        {
            return 2;
        }
        // By default, a piece can move to an immediately adjacent tile
        return 1;
    }

    /// <summary>
    /// Returns the destination tiles reachable by the movement behavior of the SourcePiece
    /// </summary>
    protected virtual List<Tile> GetMovementFromBehavior()
    {
        // By default, a piece is able to move to any empty adjacent tile
        return GetAdjacentEmptyTiles(Board.GetPieceTileById(SourcePiece.Id).Pos, GetMovementDistance());
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

    /// <summary>
    /// Returns the tiles targetable based on the targeting behavior of the SourcePiece
    /// </summary>
    protected virtual List<Tile> GetTargetsFromBehavior()
    {
        // Default behavior is to target itself
        return new List<Tile> { Board.GetPieceTileById(SourcePiece.Id) };
    }

    /// <summary>
    /// Returns every tile containing a piece protected by an opposing Protector ability
    /// </summary>
    protected HashSet<Tile> GetProtectedOpponentTiles()
    {
        // Passive ability : the protector prevents itself as well as its adjacent allies to be moved by opposing pieces abilities
        // We use a HashSet since it prevent duplicate addition natively
        HashSet<Tile> protectedOpponents = new HashSet<Tile>();
        PlayerColor opponentColor = SourcePiece.Color.GetOpposite();
        foreach (Tile opposingProtectorTile in Board.FindTilesWithMatchingPiece(opponentColor, PieceKind.Protector))
        {
            // We add every protector to the list
            protectedOpponents.Add(opposingProtectorTile);
            // Then every ally adjacent to each protector
            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                Tile? adjacentTile = Board.FindAdjacentTile(opposingProtectorTile, direction);
                if (adjacentTile is not null && adjacentTile.Piece is not null && adjacentTile.Piece.Color == opponentColor)
                {
                    protectedOpponents.Add(adjacentTile);
                }
            }
        }
        return protectedOpponents;
    }

    /// <summary>
    /// Returns a list containing only the valid targets from the allTargets parameter
    /// </summary>
    protected virtual List<Tile> FilterOutInvalidTargets(List<Tile> allTargets)
    {
        List<Tile> validTargets = new List<Tile>();
        HashSet<Tile> protectedOpponents = GetProtectedOpponentTiles();
        foreach (Tile targetTile in allTargets)
        {
            // Since the default target ability behavior is to move the target, we filter out every
            // tile protected from targeted movement or without a valid destination
            if (!protectedOpponents.Contains(targetTile) && GetTargetMovement(targetTile).Count > 0)
            {
                validTargets.Add(targetTile);
            }
        }
        return validTargets;
    }

    /// <summary>
    /// Returns the pieces tiles targetable by the SourcePiece Active ability
    /// This function should only be overridden to change the whole targeting logic, to implement
    /// a different targeting behavior, consider overriding "GetTargetsFromBehavior"
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when SourcePiece has no active ability. This indicates a programming error.
    /// </exception>
    public virtual List<Tile> GetTargets()
    {        
        if (!HasActiveAbility()){
            throw new InvalidOperationException("Cannot target without an active ability");
        }
        return FilterOutInvalidTargets(GetTargetsFromBehavior());
    }

    /// <summary>
    /// Returns the target destination tiles reachable by the target movement behavior of the SourcePiece
    /// </summary>
    protected virtual List<Tile> GetTargetMovementFromBehavior(Piece targetPiece, Position targetOriginPos)
    {
        // By default, the target piece can be moved to any empty adjacent tile
        return GetAdjacentEmptyTiles(targetOriginPos, 1);
    }

    /// <summary>
    /// Returns the Ability action matching the target movement described by the SourcePiece and parameters
    /// </summary>
    protected virtual PieceAction GetTargetMovementAction(Piece targetPiece, Position targetOriginPos, Position targetDestPos, Position sourceOriginPos)
    {
        // By default, a target movement action moves only the target
        return PieceAction.BuildAbilityAction(SourcePiece, sourceOriginPos, null, targetPiece, targetOriginPos, targetDestPos);
    }

    /// <summary>
    /// Returns a list containing only the valid target destinations from the allTargetMovement parameter
    /// </summary>
    protected List<Tile> FilterOutInvalidTargetMovement(Piece targetPiece, Position targetOriginPos, List<Tile> allTargetMovement)
    {
        List<Tile> validMovement = new List<Tile>();
        Position sourceOriginPos = Board.GetPieceTileById(SourcePiece.Id).Pos;
        foreach (Tile targetDestTile in allTargetMovement)
        {
            if (!IsInvalidAction(GetTargetMovementAction(targetPiece, targetOriginPos, targetDestTile.Pos, sourceOriginPos)))
            {
                validMovement.Add(targetDestTile);
            }
        }
        return validMovement;
    }

    /// <summary>
    /// Returns the destinations tiles reachable by the targetTile when using an Ability action.
    /// This function should only be overridden to change the whole target movement logic, to implement
    /// a different target movement behavior, consider overriding "GetTargetMovementFromBehavior"
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when :
    /// 1. SourceToken has no active ability
    /// 2. No target piece could be found on the targetTile.
    /// Both of this exception indicate a programming error.
    /// </exception>
    public virtual List<Tile> GetTargetMovement(Tile targetTile)
    {   
        if (!HasActiveAbility()){
            throw new InvalidOperationException("Cannot initialie a target movement without an active ability");
        }
        // A piece is required to initiate a target movement
        Piece? targetPiece = targetTile.Piece;
        if (targetPiece is null)
        {
            throw new InvalidOperationException("No target piece found on the targeted tile");
        }
        Position targetOriginPos = targetTile.Pos;
        return FilterOutInvalidTargetMovement(targetPiece, targetOriginPos, GetTargetMovementFromBehavior(targetPiece, targetOriginPos));
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