namespace LeadersBoardGame.GameLogic.HistoryEntries.Phases;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class RecruitmentPhase : ISegment, IPhase
{
    public List<IGameAction> Actions => throw new System.NotImplementedException();

    public IGameAction StartAction => throw new System.NotImplementedException();

    public IGameAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.RecruitmentPhase;
}