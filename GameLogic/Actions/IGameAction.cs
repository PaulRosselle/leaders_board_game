using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public interface IGameAction
{
    public GameActionType ActionType { get; }
}