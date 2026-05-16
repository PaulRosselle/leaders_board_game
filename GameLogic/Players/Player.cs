namespace LeadersBoardGame.GameLogic.Players;

using LeadersBoardGame.GameLogic.Cards;

public class Player
{
    public PlayerColor Color { get; }
    public LeaderKind Leader { get; }
    public string Name { get; }

    public Player(PlayerColor color, LeaderKind leader, string name)
    {
        Color = color;
        Leader = leader;
        Name = name;
    }
}