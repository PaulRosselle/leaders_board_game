using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Handlers;

public class BanishmentActionHandler : IActionHandler
{
    private Game _game;
    public Game Game => _game;
    public BanishmentAction Action { get; }

    public BanishmentActionHandler(Game game, BanishmentAction action)
    {
        _game = game;
        Action = action;
    }

    public void DoAction()
    {
        _game.RecruitableCards.Remove(Action.Card);
        _game.History.Add(Action);
    }

    public void UndoAction()
    {
        _game.RecruitableCards.Add(Action.Card);
        _game.History.Remove(Action);
    }
}