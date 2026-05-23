using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Player
{
    public TeamColor Color { get; }
    public string Name { get; }

    public Player(TeamColor color, string name)
    {
        Color = color;
        Name = name;
    }
}