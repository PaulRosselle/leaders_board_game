namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;
using LeadersBoardGame.GameLogic.HistoryEntries.Segments;

public static class GameHistoryQuery
{
    // TODO - comment
    public static IHistoryEntry GetLastEntry(GameHistory history)
    {
        if (history.Entries.Count == 0)
        {
            throw new InvalidOperationException("Impossible to get the last history entry with an empty history");
        }
        return history.Entries[^1];
    }

    // TODO - comment
    public static Turn? GetCurrentTurn(GameHistory history)
    {
        if (history.Entries.Count > 0 && history.Entries[^1] is Turn turn)
        {
            return turn;
        }
        return null;
    }

    /// <summary>
    /// Returns the phase currently in progress found in the last entry of the history.
    /// Returns null if there were no phase in progress
    /// </summary>
    public static IPhase? GetCurrentPhase(GameHistory history)
    {
        return GetLastPhase(history, false, s => s.StartAction is not null && s.EndAction is null);
    }

    /// <summary>
    /// Returns the last ended phase found in the last entry of the history.
    /// Returns null if there were no phase in progress
    /// </summary>
    public static IPhase? GetLastEndedPhase(GameHistory history)
    {
        return GetLastPhase(history, true, s => s.StartAction is not null && s.EndAction is not null);
    }

    /// <summary>
    /// Searches the last entry of the history for a phase matching the given predicate.
    /// If the last entry is a turn, its phases are iterated in order,
    /// optionally reversed, and the first match is returned.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown if the last history entry is of an unknown type
    /// </exception>
    private static IPhase? GetLastPhase(GameHistory history, bool reverseSearchInTurn, Func<Segment, bool> predicate)
    {
        if (history.Entries.Count == 0)
        {
            return null;
        }

        IHistoryEntry lastEntry = history.Entries[^1];
        // If the last history entry is a phase, we can return it immediately
        if (lastEntry is IPhase phase)
        {
            return phase;
        }
            
        // If the last histoiry entry is a turn, we return the first one matching the predicate
        if (lastEntry is Turn turn)
        {
            List<IPhase> phases = turn.GetPhasesInOrder();
            if (reverseSearchInTurn)
            {
                phases.Reverse();
            }
            foreach (IPhase turnPhase in phases)
            {
                if (turnPhase is Segment segment && predicate(segment))
                {
                    return turnPhase;
                }
            }
            return null;
        }
        // If this change, this algorithm would need to be updated to take into account the new cases
        throw new NotSupportedException("History entries can only be turns or phases");
    }

    /// <summary>
    /// Returns the team color of the last history entry.
    /// Returns null if the history is empty
    /// </summary>
    public static TeamColor? GetLastEntryTeam(GameHistory history)
    {
        return history.Entries.Count > 0 ? history.Entries[^1].Team : null;
    }

    /// <summary>
    /// Returns the player with the matching team color
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no player with the given teamColor could be found
    /// </exception>
    public static Player GetPlayerFromTeam(GameHistory history, TeamColor teamColor)
    {
        foreach (Player player in history.Config.Players)
        {
            if (player.Team == teamColor)
            {
                return player;
            }
        }
        throw new InvalidOperationException($"No player found for team {teamColor}");
    }
}