namespace LeadersBoardGame.GameLogic.Resolvers.Characters;

using LeadersBoardGame.GameLogic.Entities;

public class DefaultCharacterActionResolver : CharacterActionResolver
{
    public DefaultCharacterActionResolver(Game game, GameHistory history, Character character) : base(game, history, character)
    {
    }
}