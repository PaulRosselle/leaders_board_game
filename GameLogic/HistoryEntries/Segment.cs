namespace LeadersBoardGame.GameLogic.HistoryEntries;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

// TODO - update documentation
public abstract class Segment
{
    public virtual TransitionTarget Transition { get; }
    public TransitionAction? StartAction { get; private set; }
    public TransitionAction? EndAction { get; private set; }

    public void Start()
    {
        StartAction = new TransitionAction(TransitionType.Start, Transition);
    }

    public void End()
    {
        StartAction = new TransitionAction(TransitionType.Start, Transition);
    }

    public bool HasStarted()
    {
        return StartAction is not null;
    }

    public bool HasEnded()
    {
        return StartAction is null;
    }
}