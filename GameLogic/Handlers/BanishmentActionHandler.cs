namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class BanishmentActionHandler : IActionHandler
{
    public Game Game { get; }
    public BanishmentAction Action { get; }

    public BanishmentActionHandler(Game game, BanishmentAction action)
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