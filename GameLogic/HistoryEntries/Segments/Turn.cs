namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class Turn : IHistoryEntry, ISegment
{
    public TeamColor Team => throw new System.NotImplementedException();

    public TransitionAction StartAction => throw new System.NotImplementedException();

    public TransitionAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.Turn;

    public TurnStartPhase TurnStartPhase { get; }

    public ActionsPhase ActionsPhase { get; }

    public RecruitmentPhase RecruitmentPhase { get; }

    public TurnEndPhase TurnEndPhase { get; }

    public Turn()
    {
        TurnStartPhase = new TurnStartPhase();
        ActionsPhase = new ActionsPhase();
        RecruitmentPhase = new RecruitmentPhase();
        TurnEndPhase = new TurnEndPhase();
    }
}