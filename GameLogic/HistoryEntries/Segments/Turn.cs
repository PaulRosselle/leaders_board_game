namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public class Turn : Segment, IHistoryEntry
{
    public TeamColor Team { get; }

    public override TransitionTarget Transition => TransitionTarget.Turn;

    public TurnStartPhase TurnStartPhase { get; }

    public ActionsPhase ActionsPhase { get; }

    public RecruitmentPhase RecruitmentPhase { get; }

    public TurnEndPhase TurnEndPhase { get; }

    public Turn(TeamColor team)
    {
        Team = team;
        TurnStartPhase = new TurnStartPhase();
        ActionsPhase = new ActionsPhase();
        RecruitmentPhase = new RecruitmentPhase();
        TurnEndPhase = new TurnEndPhase();
    }

    /// <summary>
    /// Returns the phases composing a turn in order
    /// </summary>
    public List<IPhase> GetPhasesInOrder()
    {
        return [TurnStartPhase, ActionsPhase, RecruitmentPhase, TurnEndPhase];
    }

    // TODO - add summary
    public List<Segment> GetPhasesAsSegments()
    {
        List<Segment> segments = [];
        foreach (IPhase phase in segments)
        {
            if (phase is Segment segment)
            {
                segments.Add(segment);
            }
        }
        return segments;
    }

    // TODO - add summary
    public Segment GetPhaseAsSegment(GamePhaseType gamePhaseType)
    {
        TransitionTarget transition = gamePhaseType.GetTransition(); 
        foreach (Segment segment in GetPhasesAsSegments())
        {
            if (segment.Transition == transition)
            {
                return segment;
            }
        }
        throw new InvalidOperationException($"No phase found matching {gamePhaseType} phase type");
    }
}