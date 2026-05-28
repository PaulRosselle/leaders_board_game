namespace LeadersBoardGame.GameLogic.HistoryEntries;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public interface ISegment
{
    public TransitionTarget Transition { get; }
    public TransitionAction StartAction { get; }
    public TransitionAction EndAction { get; }
}