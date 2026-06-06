namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class BanishmentPhase : Segment, IHistoryEntry, IPhase
{
    public TeamColor Team { get; }

    public List<IGameAction> Actions { get; }

    public override TransitionTarget Transition => TransitionTarget.BanishmentPhase;

    public BanishmentPhase(TeamColor team)
    {
        Team = team;
        Actions = [];
    }
}