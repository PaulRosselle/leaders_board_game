namespace LeadersBoardGame.GameLogic.HistoryEntries.Phases;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class Turn : IHistoryEntry, ISegment
{
    public TeamColor Team => throw new System.NotImplementedException();

    public IGameAction StartAction => throw new System.NotImplementedException();

    public IGameAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.Turn;

    public TurnStartPhase TurnStart { get; }

    public ActionsPhase Actions { get; }

    public RecruitmentPhase Recruitment { get; }

    public TurnEndPhase TurnEnd { get; }

    public Turn()
    {
        TurnStart = new TurnStartPhase();
        Actions = new ActionsPhase();
        Recruitment = new RecruitmentPhase();
        TurnEnd = new TurnEndPhase();
    }
}