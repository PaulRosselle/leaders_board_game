namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class TurnEndPhase : Segment, IPhase
{
    public List<IGameAction> Actions { get; }

    public override TransitionTarget Transition => TransitionTarget.TurnEndPhase;

    public TurnEndPhase()
    {
        Actions = [];
    }
}