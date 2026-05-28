namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class RecruitmentActionHandler : IActionHandler
{
    private readonly Game _game;
    public Game Game => _game;
    public RecruitmentAction Action { get; }

    public RecruitmentActionHandler(Game game, RecruitmentAction action)
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