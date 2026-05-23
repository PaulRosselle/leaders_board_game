using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Handlers;

public interface IActionHandler
{
    Game Game { get; }
    
    void DoAction();
    void UndoAction();
}