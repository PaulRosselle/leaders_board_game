namespace LeadersBoardGame.GameLogic.Factories;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;
using LeadersBoardGame.GameLogic.Resolvers;
using LeadersBoardGame.GameLogic.Resolvers.Characters;

public static class CharacterActionResolverFactory
{
    public static CharacterActionResolver Create(Game game, GameHistory history, Character character)
    {
        if (character.CharacterType.GetCharacterCard().IsLeader())
        {
            return new LeaderCharacterActionResolver(game, history, character);
        }
        return character.CharacterType switch
        {
            CharacterType.Acrobat => new AcrobatCharacterActionResolver(game, history, character),
            CharacterType.Brewmaster => new BrewmasterCharacterActionResolver(game, history, character),
            CharacterType.Bruiser => new BruiserCharacterActionResolver(game, history, character),
            CharacterType.ClawLauncher => new ClawLauncherCharacterActionResolver(game, history, character),
            CharacterType.Illusionist => new IllusionistCharacterActionResolver(game, history, character),
            CharacterType.Manipulator => new ManipulatorCharacterActionResolver(game, history, character),
            CharacterType.Nemesis => new NemesisCharacterActionResolver(game, history, character),
            CharacterType.Rider => new RiderCharacterActionResolver(game, history, character),
            CharacterType.RoyalGuard => new RoyalGuardCharacterActionResolver(game, history, character),
            CharacterType.Wanderer => new WandererCharacterActionResolver(game, history, character),
            _ => new DefaultCharacterActionResolver(game, history, character),
        };
    }
}