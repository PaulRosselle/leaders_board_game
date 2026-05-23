using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Character
{
    public int Id { get; }
    public CharacterType CharacterType { get; }
    public TeamColor Color { get; }

    public Character(int id, CharacterType type, TeamColor color)
    {
        Id = id;
        Color = color;
        CharacterType = type;
    }

    public Character(Character refCharacter) : this(refCharacter.Id, refCharacter.CharacterType, refCharacter.Color)
    {
    }
}