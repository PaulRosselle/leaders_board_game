namespace LeadersBoardGame.GameLogic.Factories;

using System;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public static class CharacterFactory
{
    public static Character Create(CharacterType characterType, TeamColor team)
    {
        return new Character(new Guid(), characterType, team);
    }

    public static Character Transform(Character originalCharacter, CharacterType characterType, TeamColor team)
    {
        return new Character(originalCharacter.Id, characterType, team);
    }
}