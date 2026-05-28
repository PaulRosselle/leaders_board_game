namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class BanishmentActionHandler : IActionHandler
{
    private readonly Game _game;
    public Game Game => _game;
    public BanishmentAction Action { get; }

    public BanishmentActionHandler(Game game, BanishmentAction action)
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