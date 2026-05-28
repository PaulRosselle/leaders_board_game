using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Actions;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Game
{
    public Player[] Players { get; }
    public Player FirstPlayer { get; }
    public Player CurrentPlayer { get; set; }
    public TurnPhase CurrentPhase {get; set; }
    public Board Board { get; }
    public GameMode GameMode { get; }
    public List<CharacterCard> RecruitableCards { get; }
    public List<IGameAction> History;

    public Game(Player[] players, Player firstPlayer, Player currentPlayer, TurnPhase currentPhase,
                Board board, GameMode gameMode, List<CharacterCard> recruitableCards, 
                List<IGameAction> history)
    {
        Players = players;
        FirstPlayer = firstPlayer;
        CurrentPlayer = currentPlayer;
        CurrentPhase = currentPhase;
        Board = board;
        GameMode = gameMode;
        RecruitableCards = recruitableCards;
        History = history;
    }
}