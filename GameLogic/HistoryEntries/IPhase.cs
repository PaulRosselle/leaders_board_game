namespace LeadersBoardGame.GameLogic.HistoryEntries;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;

public interface IPhase
{
    public List<IGameAction> Actions { get; }
}