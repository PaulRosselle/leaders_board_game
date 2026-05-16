namespace LeadersBoardGame.GameLogic.Boards;

using System;
using System.Collections.Generic;

public enum Direction
{
    Top,
    TopRight,
    BottomRight,
    Bottom,
    BottomLeft,
    TopLeft
}

public static class DirectionMethods
{
    public static Direction GetOpposite(this Direction direction) => direction switch
    {
        Direction.Top => Direction.Bottom,
        Direction.TopRight => Direction.BottomLeft,
        Direction.BottomRight => Direction.TopLeft,
        Direction.Bottom => Direction.Top,
        Direction.BottomLeft => Direction.TopRight,
        Direction.TopLeft => Direction.BottomRight,
        _ => throw new InvalidOperationException($"No opposite found for direction {direction}"),
    };

    public static bool IsSameColumn(this Direction direction)
    {
        return direction == Direction.Top || direction == Direction.Bottom;
    }

    public static bool IsTop(this Direction direction)
    {
        return direction == Direction.Top || direction == Direction.TopRight || direction == Direction.TopLeft;
    }

    public static bool IsLeft(this Direction direction)
    {
        return direction == Direction.TopLeft || direction == Direction.BottomLeft;
    }

    public static Direction GetNext(this Direction direction, bool clockwise)
    {
        // We add every direction in clockwise order.
        // For the function to cycle automatically, we add again the first direction at the end of the list
        List<Direction> directions = [
            Direction.Top, Direction.TopRight, Direction.BottomRight, 
            Direction.Bottom, Direction.BottomLeft, Direction.TopLeft, Direction.Top  
        ];
        // We just have to reverse the list order to get the next direction counter-clockwise
        if (!clockwise)
        {
            directions.Reverse();
        }
        // Since the last direction is always a repetition of the first one, we are
        // sure to never get out of bounds by getting the next direction with indexOf + 1
        return directions[directions.IndexOf(direction) + 1];
    }
}