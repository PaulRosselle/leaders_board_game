namespace LeadersBoardGame.GameLogic.Entities;

public class CharacterContext
{
    public Character Character { get; }
    public CharacterState State { get; }
    public Position Position { get; }

    public CharacterContext(Character character, CharacterState state, Position position)
    {
        Character = character;
        State = state;
        Position = position;
    }
}