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
        throw new NotImplementedException();
    }

    public void UndoAction()
    {
        throw new NotImplementedException();
    }
}