namespace LeadersBoardGame.GameLogic.Entities;

using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;

public class Game
{
    public Board Board { get; }
    public List<CharacterCard> RecruitableCards { get; }
    public List<Character> RecruitedCharacters { get; }
    public List<CharacterCard> BannishedCards { get; }

    public Game(Board board, List<CharacterCard> recruitableCards, 
                List<Character> recruitedCharacters, List<CharacterCard> bannishedCards)
    {
        Board = board;
        RecruitableCards = recruitableCards;
        RecruitedCharacters = recruitedCharacters;
        BannishedCards = bannishedCards;
    }
}