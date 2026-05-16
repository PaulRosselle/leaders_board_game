namespace LeadersBoardGame.GameLogic.Actions;

using System;
using System.Collections.Generic;

public class PieceActionHistory
{
    public List<List<PieceAction>> ActionsPerTurn { get; init; }

    public PieceActionHistory()
    {
        ActionsPerTurn = new List<List<PieceAction>>();
    }

    public PieceActionHistory(PieceActionHistory refActionHistory) : this()
    {
        foreach (List<PieceAction> refTurnActions in refActionHistory.ActionsPerTurn)
        {
            List<PieceAction> turnActions = new List<PieceAction>(refTurnActions.Capacity);
            foreach (PieceAction refAction in refTurnActions)
            {
                turnActions.Add(new PieceAction(refAction));
            }
        }
    }
    
    public List<PieceAction> GetLastTurnActions()
    {
        if (ActionsPerTurn.Count == 0)
        {
            throw new InvalidOperationException("Couldn't get last turn action : no turn registered");
        }
        return ActionsPerTurn[^1];
    }

    public PieceAction GetLastAction()
    {
        List<PieceAction> lastTurnActions = GetLastTurnActions();
        if (lastTurnActions.Count == 0)
        {
            throw new InvalidOperationException("Couldn't get last action : no action registered");
        }
        return lastTurnActions[^1];
    }

    public void AddAction(PieceAction action)
    {
        GetLastTurnActions().Add(action);
    }

    public void RemoveAction(PieceAction action)
    {
        if (GetLastTurnActions().Remove(action))
        {
            throw new InvalidOperationException("Couldn't remove action");
        }
    }

    public void RemoveLastAction()
    {
        RemoveAction(GetLastAction());
    }

    public void StartNewTurn()
    {
        ActionsPerTurn.Add(new List<PieceAction>());
    }
}