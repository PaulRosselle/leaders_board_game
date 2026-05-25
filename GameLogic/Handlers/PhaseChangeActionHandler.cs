using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Handlers;

public class PhaseChangeActionHandler : IActionHandler
{
    private readonly Game _game;
    public Game Game => _game;
    public PhaseChangeAction Action { get; }

    public PhaseChangeActionHandler(Game game, PhaseChangeAction action)
    {
        _game = game;
        Action = action;
    }

    public void DoAction()
    {
        _game.CurrentPhase = Action.NewPhase;
        _game.History.Add(Action);
    }

    public void UndoAction()
    {
        _game.CurrentPhase = Action.PreviousPhase;
        _game.History.Remove(Action);
    }
}