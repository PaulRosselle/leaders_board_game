namespace LeadersBoardGame.GameLogic.Enums;

using System;

public enum CharacterType
{
    Acrobat,
    Archer,
    Assassin,
    Brewmaster,
    Bruiser,
    ClawLauncher,
    Cub,
    Hermit,
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

public static class CharacterTypeMethods
{
    public static CharacterCard GetCharacterCard(this CharacterType characterType) => characterType switch
    {
        CharacterType.Acrobat => CharacterCard.Acrobat,
        CharacterType.Archer => CharacterCard.Archer,
        CharacterType.Assassin => CharacterCard.Assassin,
        CharacterType.Brewmaster => CharacterCard.Brewmaster,
        CharacterType.Bruiser => CharacterCard.Bruiser,
        CharacterType.ClawLauncher => CharacterCard.ClawLauncher,
        CharacterType.Hermit or CharacterType.Cub => CharacterCard.HermitAndCub,
        CharacterType.Illusionist => CharacterCard.Illusionist,
        CharacterType.Jailer => CharacterCard.Jailer,
        CharacterType.LeaderKing => CharacterCard.LeaderKing,
        CharacterType.LeaderQueen => CharacterCard.LeaderQueen,
        CharacterType.Manipulator => CharacterCard.Manipulator,
        CharacterType.Nemesis => CharacterCard.Nemesis,
        CharacterType.Protector => CharacterCard.Protector,
        CharacterType.Rider => CharacterCard.Rider,
        CharacterType.RoyalGuard => CharacterCard.RoyalGuard,
        CharacterType.Vizier => CharacterCard.Vizier,
        CharacterType.Wanderer => CharacterCard.Wanderer,
        _ => throw new InvalidOperationException($"No card found for character {characterType}"),
    };
}