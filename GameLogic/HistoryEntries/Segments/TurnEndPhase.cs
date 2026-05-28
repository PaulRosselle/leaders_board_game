namespace LeadersBoardGame.GameLogic.HistoryEntries.Phases;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class TurnEndPhase : ISegment, IPhase
{
    public List<IGameAction> Actions => throw new System.NotImplementedException();

    public TransitionAction StartAction => throw new System.NotImplementedException();

    public TransitionAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.TurnEndPhase;
}