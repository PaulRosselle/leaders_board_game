namespace LeadersBoardGame.GameLogic.Actions;

using LeadersBoardGame.GameLogic.Enums;

public interface IGameAction
{
    public GameActionType ActionType { get; }
}