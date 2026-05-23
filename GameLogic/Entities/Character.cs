using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Character
{
    public int Id { get; }
    public CharacterType Type { get; set; }
    public TeamColor Color { get; set; }

    public Character(int id, CharacterType type, TeamColor color)
    {
        Id = id;
        Color = color;
        Type = type;
    }

    public Character(Character refCharacter) : this(refCharacter.Id, refCharacter.Type, refCharacter.Color)
    {
    }
}