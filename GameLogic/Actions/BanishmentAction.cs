using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Actions;

public class BanishmentAction : IGameAction
{
    public GameActionType ActionType => GameActionType.Banishment;

    public CharacterCard Card { get; }
    public TeamColor Team { get; }

    public BanishmentAction(CharacterCard card, TeamColor team)
    {
        Card = card;
        Team = team;
    }
}