namespace LeadersBoardGame.GameLogic.Actions;

using LeadersBoardGame.GameLogic.Entities;

public class CharacterActionTarget
{
    public Character Character { get; }
    public Position? OriginPos { get; }
    public Position? DestPos { get; }

    public CharacterActionTarget(Character character, Position? originPos, Position? destPos)
    {
        Character = character;
        OriginPos = originPos;
        DestPos = destPos;
    }
}