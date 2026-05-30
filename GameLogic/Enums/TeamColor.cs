namespace LeadersBoardGame.GameLogic.Enums;

public enum TeamColor
{
    Black,
    White
}

public static class TeamColorExtension
{
    public static TeamColor GetOpposite(this TeamColor teamColor)
    {
        if (teamColor == TeamColor.Black)
        {
            return TeamColor.White;
        }
        return TeamColor.Black;
    }
}