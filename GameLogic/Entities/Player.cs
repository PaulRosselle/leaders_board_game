using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Player
{
    public TeamColor Team { get; }
    public string Name { get; }

    public Player(TeamColor team, string name)
    {
        Team = team;
        Name = name;
    }
}