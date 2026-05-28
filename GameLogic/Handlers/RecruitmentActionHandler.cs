namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class RecruitmentActionHandler : IActionHandler
{
    public Game Game { get; }

    public GameHistory History { get; }

    public RecruitmentAction Action { get; }

    public RecruitmentActionHandler(Game game, GameHistory history, RecruitmentAction action)
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