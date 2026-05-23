namespace LeadersBoardGame.GameLogic.Entities;

public class Cell
{
    public Position Pos { get; }
    public Character? Character { get; set; }

    public Cell(Position pos)
    {
        Pos = pos;
    }

    public Cell(Cell refCell) : this(new Position(refCell.Pos))
    {
        Character = refCell.Character is null ? null : new Character(refCell.Character);
    }
}