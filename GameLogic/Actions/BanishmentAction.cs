using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public class BanishmentAction : IGameAction
{
    public GameActionType ActionType => GameActionType.Banishment;

    public CharacterCard Card { get; }
    public Player Author { get; }

    public BanishmentAction(CharacterCard card, Player author)
    {
        Card = card;
        Author = author;
    }
}