namespace LeadersBoardGame.GameLogic.Factories;

using System;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public static class CharacterFactory
{
    public static Character Create(CharacterType characterType, TeamColor teamColor)
    {
        return new Character(new Guid(), characterType, teamColor);
    }

    public static Character Transform(Character originalCharacter, CharacterType characterType, TeamColor teamColor)
    {
        return new Character(originalCharacter.Id, characterType, teamColor);
    }
}