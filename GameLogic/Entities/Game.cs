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
    public List<CharacterCard> RecruitableCards { get; }
    public Dictionary<Character, CharacterState> CharacterStates { get; }
    public List<IGameAction> History;

    public Game(Player[] players, Player firstPlayer, Player currentPlayer, TurnPhase currentPhase, Board board, 
                List<CharacterCard> recruitableCards, Dictionary<Character, CharacterState> characterStates, 
                List<IGameAction> history)
    {
        Players = players;
        FirstPlayer = firstPlayer;
        CurrentPlayer = currentPlayer;
        CurrentPhase = currentPhase;
        Board = board;
        RecruitableCards = recruitableCards;
        CharacterStates = characterStates;
        History = history;
    }
}