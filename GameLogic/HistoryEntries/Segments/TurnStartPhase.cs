namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class TurnStartPhase : ISegment, IPhase
{
    public List<IGameAction> Actions => throw new System.NotImplementedException();

    public TransitionAction StartAction => throw new System.NotImplementedException();

    public TransitionAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.TurnStartPhase;
}