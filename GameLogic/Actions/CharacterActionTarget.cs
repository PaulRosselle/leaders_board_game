using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Actions;

public class CharacterActionDetail
{
    public Character Character { get; }
    public Position? OriginPos { get; }
    public Position? DestPos { get; }

    public CharacterActionDetail(Character character, Position? originPos, Position? destPos)
    {
        Character = character;
        OriginPos = originPos;
        DestPos = destPos;
    }
}