using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Handlers;

public interface IActionHandler
{
    Game Game { get; }
    GameHistory History { get; }
    
    void DoAction();
    void UndoAction();
}