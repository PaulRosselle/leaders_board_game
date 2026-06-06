namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class RecruitmentPhase : Segment, IPhase
{
    public List<IGameAction> Actions { get; }

    public override TransitionTarget Transition => TransitionTarget.RecruitmentPhase;

    public RecruitmentPhase()
    {
        Actions = [];
    }
}