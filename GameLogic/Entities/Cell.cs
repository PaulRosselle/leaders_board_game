using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Enums;

namespace LeadersBoardGame.GameLogic.Entities;

public class Cell
{
    public Position Pos { get; }

    public Character? Character { get; set; }

    public Dictionary<Direction, Cell> AdjacentCells { get; }

    public Cell(Position pos)
    {
        Pos = pos;
        AdjacentCells = [];
    }

    /// <summary>
    /// Sets the adjacent cell to the cell in a given direction. 
    /// Should not be used outside of board initialization
    /// </summary>
    public void SetAdjacentCell(Direction direction, Cell adjacentCell)
    {
        AdjacentCells.Add(direction, adjacentCell);
    }
}