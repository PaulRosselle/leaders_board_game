namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class TransitionActionHandler : IActionHandler
{
    private readonly Game _game;
    public Game Game => _game;
    public TransitionAction Action { get; }

    public TransitionActionHandler(Game game, TransitionAction action)
    {
        _game = game;
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