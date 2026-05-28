namespace LeadersBoardGame.GameLogic.HistoryEntries;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public interface ISegment
{
    public TransitionTarget Transition { get; }
    public IGameAction StartAction { get; }
    public IGameAction EndAction { get; }
}