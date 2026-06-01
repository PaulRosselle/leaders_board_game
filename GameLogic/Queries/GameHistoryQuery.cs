namespace LeadersBoardGame.GameLogic.Queries;

using System;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;

public static class GameHistoryQuery
{
    public static IHistoryEntry GetLastEntry(GameHistory history)
    {
        if (history.Entries.Count == 0)
        {
            throw new InvalidOperationException("Impossible to get the last history entry with an empty history");
        }
        return history.Entries[^1];
    }

    /// <summary>
    /// Returns the in progress phase found in the last entry of the history.
    /// Returns null if there were no phase in progress
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown if the last history entry is of an unknown type
    /// </exception>
    public static IPhase? GetCurrentPhase(GameHistory history)
    {
        // No current phase without entries in the history
        if (history.Entries.Count == 0)
        {
            return null;
        }

        // The current phase correspond (or is within) the last history entry
        IHistoryEntry lastEntry = history.Entries[^1];
        // If the last history entry is a phase, we can return it immediately
        if (lastEntry is IPhase phase)
        {
            return phase;
        }
        // If the last histoiry entry is a turn, we must check which phase in the turn is in progress
        if (lastEntry is Turn turn)
        {
            foreach (IPhase turnPhase in turn.GetPhasesInOrder())
            {
                // A phase must have started but not ended to be considered in progress
                if (turnPhase is ISegment segment && 
                    segment.StartAction is not null && segment.EndAction is null)
                {
                    return turnPhase;
                }
            }
            return null;
        }
        // History entries can only be instances of Turn or IPhase at the moment.
        // If this change, this algorithm would need to be updated to take into account the new cases
        throw new NotSupportedException("History entries can only be turns or phases");
    }

    /// <summary>
    /// Returns the team color of the last history entry.
    /// Returns null if the history is empty
    /// </summary>
    public static TeamColor? GetCurrentEntryTeam(GameHistory history)
    {
        return history.Entries.Count > 0 ? history.Entries[^1].Team : null;
    }
}