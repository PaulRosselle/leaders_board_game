namespace LeadersBoardGame.GameLogic.Entities;

using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

public class GameConfig
{
    public Player[] Players { get; }
    public Player FirstPlayer { get; }
    public GameMode GameMode { get; }
    public CharacterCard[] InitialRecruitableCards { get; }
    public IGameAction[] InitialPlacements { get; }

    public GameConfig(Player[] players, Player firstPlayer, GameMode gameMode,
                      CharacterCard[] initialRecruitableCards, IGameAction[] initialPlacements)
    {
        Players = players;
        FirstPlayer = firstPlayer;
        GameMode = gameMode;
        InitialRecruitableCards = initialRecruitableCards;
        InitialPlacements = initialPlacements;
    }
}