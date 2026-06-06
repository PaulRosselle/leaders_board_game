namespace LeadersBoardGame.GameLogic.Queries;

using System;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.HistoryEntries;

// TODO - add to documentation
public static class PhaseTransitionQuery
{
    private static GamePhase GetFirstPhase(GameHistory history, Game game)
    {
        // TODO - comment
        if (history.Config.GameMode == GameMode.Discovery)
        {
            return new GamePhase(GamePhaseType.TurnStart, history.Config.FirstPlayer);
        }
        // TODO - comment
        if (history.Config.GameMode == GameMode.Strategist)
        {
            return new GamePhase(GamePhaseType.Banishment, GameHistoryQuery.GetPlayerFromTeam(history, history.Config.FirstPlayer.Team.GetOpposite()));
        }
        // If this change, this algorithm would need to be updated to take into account the new cases
        throw new InvalidOperationException("Game modes are limited to Discovery and Strategist");
    }

    public static GamePhase GetNextPhase(GameHistory history, Game game)
    {
        if (history.Entries.Count == 0)
        {
            return GetFirstPhase(history, game);
        }
        
        TeamColor lastPhaseTeam = GetLastEntryTeamOrThrow(history);
        GamePhaseType nextPhaseType = GetNextPhaseType(history, game, GetLastEndedSegmentOrThrow(history).Transition, lastPhaseTeam);
        TeamColor nextPhaseTeam = GetNextPhaseTeam(nextPhaseType, lastPhaseTeam);
        return new GamePhase(nextPhaseType, GameHistoryQuery.GetPlayerFromTeam(history, nextPhaseTeam));
    }

    private static Segment GetLastEndedSegmentOrThrow(GameHistory history)
    {
        IPhase? lastPhase = GameHistoryQuery.GetLastEndedPhase(history);
        if (lastPhase is not Segment lastEndedSegment)
            throw new InvalidOperationException("The last game phase must correspond to a segment");
        return lastEndedSegment;
    }

    private static TeamColor GetLastEntryTeamOrThrow(GameHistory history)
    {
        TeamColor? team = GameHistoryQuery.GetLastEntryTeam(history);
        if (team is not TeamColor lastTeam)
            throw new InvalidOperationException("The last game phase must have an associated team");
        return lastTeam;
    }

    private static GamePhaseType GetNextPhaseType(GameHistory history, Game game, TransitionTarget transition, TeamColor currentTeam)
    {
        TeamColor oppositeTeam = currentTeam.GetOpposite();
        return transition switch
        {
            TransitionTarget.TurnStartPhase => GamePhaseType.Actions,
            TransitionTarget.RecruitmentPhase => GamePhaseType.TurnEnd,
            // TODO - comment
            TransitionTarget.ActionsPhase => 
                RecruitmentQuery.CanRecruit(history, game, oppositeTeam) ? GamePhaseType.Recruitment : GamePhaseType.TurnEnd,
            // TODO - comment
            TransitionTarget.TurnEndPhase or TransitionTarget.BanishmentPhase => 
                BanishmentQuery.CanBanish(game) ? GamePhaseType.Banishment : GamePhaseType.TurnStart,
            _ => throw new InvalidOperationException($"\"{transition}\" is not a valid transition")
        };
    }

    private static TeamColor GetNextPhaseTeam(GamePhaseType nextPhaseType, TeamColor currentTeam)
    {
        // TODO - comment
        if (nextPhaseType == GamePhaseType.TurnStart || nextPhaseType == GamePhaseType.Banishment)
        {
            return currentTeam.GetOpposite();
        }
        return currentTeam;
    }
}