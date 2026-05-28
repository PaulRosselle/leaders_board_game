using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Player
{
    public TeamColor Color { get; }
    public string Team { get; }

    public Player(TeamColor color, string team)
    {
        Color = color;
        Team = team;
    }
}