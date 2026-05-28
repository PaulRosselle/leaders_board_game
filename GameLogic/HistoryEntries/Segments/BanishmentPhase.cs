namespace LeadersBoardGame.GameLogic.HistoryEntries.Segments;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class BanishmentPhase : IHistoryEntry, ISegment, IPhase
{
    public TeamColor Team => throw new System.NotImplementedException();

    public List<IGameAction> Actions => throw new System.NotImplementedException();

    public TransitionAction StartAction => throw new System.NotImplementedException();

    public TransitionAction EndAction => throw new System.NotImplementedException();

    public TransitionTarget Transition => TransitionTarget.BanishmentPhase;
}