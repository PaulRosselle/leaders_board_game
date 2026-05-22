namespace LeadersBoardGame.GameLogic.Enums;

using System;

public enum CharacterCard
{
    Acrobat,
    Archer,
    Assassin,
    Brewmaster,
    Bruiser,
    ClawLauncher,
    HermitAndCub,
    Illusionist,
    Jailer,
    LeaderKing,
    LeaderQueen,
    Manipulator,
    Nemesis,
    Protector,
    Rider,
    RoyalGuard,
    Vizier,
    Wanderer
}

public static class CharacterCardMethods
{
    public static AbilityType[] GetAbilityTypes(this CharacterCard characterCard) => characterCard switch
    {
        CharacterCard.Acrobat => [AbilityType.Active],
        CharacterCard.Archer => [AbilityType.Passive],
        CharacterCard.Assassin => [AbilityType.Passive],
        CharacterCard.Brewmaster => [AbilityType.Active],
        CharacterCard.Bruiser => [AbilityType.Active],
        CharacterCard.ClawLauncher => [AbilityType.Active],
        CharacterCard.HermitAndCub => [AbilityType.Special],
        CharacterCard.Illusionist => [AbilityType.Active],
        CharacterCard.Jailer => [AbilityType.Passive],
        CharacterCard.LeaderKing or CharacterCard.LeaderQueen => [],
        CharacterCard.Manipulator => [AbilityType.Active],
        CharacterCard.Nemesis => [AbilityType.Special],
        CharacterCard.Protector => [AbilityType.Passive],
        CharacterCard.Rider => [AbilityType.Active],
        CharacterCard.RoyalGuard => [AbilityType.Active],
        CharacterCard.Vizier => [AbilityType.Passive],
        CharacterCard.Wanderer => [AbilityType.Active],
        _ => throw new InvalidOperationException($"No ability types found for card {characterCard}"),
    };

    public static bool IsLeader(this CharacterCard characterCard)
    {
        return characterCard == CharacterCard.LeaderKing || characterCard == CharacterCard.LeaderQueen;
    }
    
    public static bool CanBeRecruited(this CharacterCard characterCard)
    {
        // Leaders start the game directly on the board and cant be recruited afterward
        return !IsLeader(characterCard);
    }
}