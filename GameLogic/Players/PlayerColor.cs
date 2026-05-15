namespace LeadersBoardGame.Players;

public enum PlayerColor
{
    Black,
    White
}

public static class PlayerColorMethods
{
    public static PlayerColor GetOpposite(this PlayerColor playerColor)
    {
        if (playerColor == PlayerColor.Black)
        {
            return PlayerColor.White;
        }
        return PlayerColor.Black;
    }
}