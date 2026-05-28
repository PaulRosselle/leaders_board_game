using System;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Character
{
    public Guid Id { get; }
    public CharacterType CharacterType { get; }
    public TeamColor Color { get; }

    public Character(Guid id, CharacterType type, TeamColor color)
    {
        Id = id;
        Color = color;
        CharacterType = type;
    }
}