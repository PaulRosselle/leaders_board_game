namespace LeadersBoardGame.GameLogic.Entities;

using System;
using LeadersBoardGame.GameLogic.Enums;

public class Character
{
    public Guid Id { get; }
    public CharacterType CharacterType { get; }
    public TeamColor Team { get; }

    public Character(Guid id, CharacterType type, TeamColor team)
    {
        Id = id;
        Team = team;
        CharacterType = type;
    }
}