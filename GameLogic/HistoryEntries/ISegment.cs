namespace LeadersBoardGame.GameLogic.HistoryEntries;

using LeadersBoardGame.GameLogic.Actions;

public interface ISegment
{
    public IGameAction StartAction { get; }
    public IGameAction EndAction { get; }
}