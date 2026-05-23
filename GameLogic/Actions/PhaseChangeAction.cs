using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public class PhaseChangeAction : IGameAction
{
    public GameActionType ActionType => GameActionType.PhaseChange;
    public TurnPhase PreviousPhase { get; }
    public TurnPhase NewPhase { get; }

    public PhaseChangeAction(TurnPhase previousPhase, TurnPhase newPhase)
    {
        PreviousPhase = previousPhase;
        NewPhase = newPhase;
    }
}