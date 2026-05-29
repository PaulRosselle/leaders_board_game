namespace LeadersBoardGame.GameLogic.Entities;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.HistoryEntries;

public class GameHistory
{
    public GameConfig Config { get; }
    public List<IHistoryEntry> Entries { get; }

    public GameHistory(GameConfig config, List<IHistoryEntry> entries)
    {
        Config = config;
        Entries = entries;
    }
}