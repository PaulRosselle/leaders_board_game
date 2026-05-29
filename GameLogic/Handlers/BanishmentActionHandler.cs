namespace LeadersBoardGame.GameLogic.Handlers;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

public class BanishmentActionHandler : IActionHandler
{
    public Game Game { get; }

    public GameHistory History { get; }

    public BanishmentAction Action { get; }


    public BanishmentActionHandler(Game game, GameHistory history, BanishmentAction action)
    {
        Game = game;
        History = history;
        Action = action;
    }

    public void DoAction()
    {
        Game.RecruitableCards.Remove(Action.Card);
        Game.BanishedCards.Add(Action.Card);
    }

    public void UndoAction()
    {
        Game.BanishedCards.Remove(Action.Card);
        Game.RecruitableCards.Add(Action.Card);
    }
}