namespace LeadersBoardGame.GameLogic.Actions;

using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public class RecruitmentAction : IGameAction
{
    public GameActionType ActionType => GameActionType.Recruitment;

    public Character Character { get; }
    public Position DestPos { get; }

    public RecruitmentAction(Character character, Position destPos)
    {
        Character = character;
        DestPos = destPos;
    }
}