using LeadersBoardGame.GameLogic.Entities;

namespace LeadersBoardGame.GameLogic.Actions;

public class CharacterActionDetail
{
    public Character Character { get; }
    public Position OriginPos { get; }
    public Position? DestPos { get; }
    public CharacterState PreviousState { get; }
    public CharacterState NewState { get; }

    public CharacterActionDetail(Character character, Position originPos, Position? destPos, CharacterState previousState, CharacterState newState)
    {
        Character = character;
        OriginPos = originPos;
        DestPos = destPos;
        PreviousState = previousState;
        NewState = newState;
    }
}