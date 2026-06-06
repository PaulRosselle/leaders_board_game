namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class ActionsPhase : Segment, IPhase
{
    public List<IGameAction> Actions { get; }

    public override TransitionTarget Transition => TransitionTarget.ActionsPhase;

    public ActionsPhase()
    {
        Actions = [];
    }
}