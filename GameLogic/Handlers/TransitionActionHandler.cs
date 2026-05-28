namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class TransitionActionHandler : IActionHandler
{
    public Game Game { get; }

    public GameHistory History { get; }

    public TransitionAction Action { get; }

    public TransitionActionHandler(Game game, GameHistory history, TransitionAction action)
    {
        Game = game;
        History = history;
        Action = action;
    }

    public void DoAction()
    {
        throw new NotImplementedException();
    }

    public void UndoAction()
    {
        throw new NotImplementedException();
    }
}