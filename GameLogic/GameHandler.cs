namespace LeadersBoardGame.GameLogic;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Factories;

public class GameHandler
{
    public Game CurrentGame { get; }

    public GameHistory CurrentHistory { get; }

    public GameHandler(GameHistory gameHistory)
    {
        CurrentGame = GameFactory.Create(gameHistory);
        CurrentHistory = gameHistory;
    }

    /// <summary>
    /// Returns the game mode for the current game
    /// </summary>
    public GameMode GetGameMode()
    {
        // TODO - add summary
        return CurrentHistory.Config.GameMode;
    }

    /// <summary>
    /// Returns the players for the current game
    /// </summary>
    public Player[] GetPlayers()
    {
        // TODO - add summary
        return CurrentHistory.Config.Players;
    }
}