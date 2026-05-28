namespace LeadersBoardGame.GameLogic.Actions;

using LeadersBoardGame.GameLogic.Enums;

public class TransitionAction : IGameAction
{
    public GameActionType ActionType => GameActionType.Transition;

    public TransitionType TransitionType { get; }

    public TransitionTarget Target { get; }

    public TransitionAction(TransitionType transitionType, TransitionTarget target)
    {
        TransitionType = transitionType;
        Target = target;
    }
}