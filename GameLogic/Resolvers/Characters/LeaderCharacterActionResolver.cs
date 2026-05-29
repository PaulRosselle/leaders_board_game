namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Queries;

public class LeaderCharacterActionResolver : CharacterActionResolver
{
    public LeaderCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }

    protected override int GetMovementMaxDistance()
    {
        // Passive ability : the vizier allows its leader to move to up to two cells per action
        if (BoardQuery.FindCellsWithMatchingCharacter(Game.Board, Character.Color, CharacterType.Vizier).Count > 0)
        {
            return 2;
        }
        // Without a vizier we use the default behavior
        return base.GetMovementMaxDistance();
    }
}