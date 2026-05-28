namespace LeadersBoardGame.GameLogic.HistoryEntries;

using LeadersBoardGame.GameLogic.Enums;

public interface IHistoryEntry
{
    public TeamColor Team { get; }
}