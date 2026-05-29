namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class TransitionActionHandler : IActionHandler
{
    public Game Game { get; }

    public TransitionAction Action { get; }

    public TransitionActionHandler(Game game, TransitionAction action)
    {
        Game = game;
        Action = action;
    }

    public void DoAction()
    {
        // Since transitions have no impact on the game projection, we have no treatment to do here.
        // This class is kept to simplify the generic approach on IGameAction
    }

    public void UndoAction()
    {
        // Since transitions have no impact on the game projection, we have no treatment to undo here.
        // This class is kept to simplify the generic approach on IGameAction
    }
}