namespace LeadersBoardGame.GameLogic.Players;

using LeadersBoardGame.GameLogic.Cards;

public class Player
{
    public PlayerColor Color { get; init; }
    public LeaderKind Leader { get; init; }
    public string Name { get; init; }

    public Player(PlayerColor color, LeaderKind leader, string name)
    {
        Color = color;
        Leader = leader;
        Name = name;
    }
}