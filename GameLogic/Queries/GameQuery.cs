namespace LeadersBoardGame.GameLogic.Queries;

using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Entities;
using LeadersBoardGame.GameLogic.Enums;

public static class GameQuery
{
    // Currently the maximum distance were a character can be taken into 
    // account for capture is 2 tiles appart from the leader (Archer passive effect)
    private static byte s_max_capture_distance = 2;
    private static byte s_leader_required_capture_value = 2;
    
    /// <summary>
    /// Returns the capture value of the character within the parameter cell based on the distance.
    /// </summary>
    private static int GetCaptureValue(Cell? cell, byte distance, TeamColor leaderColor)
    {
        // A null or empty cell doesn't have a capture value and only opponent characters are taken into account
        if (cell is null || cell.Character is null || cell.Character.Color == leaderColor)
        {
            return 0;
        }

        return cell.Character.CharacterType switch
        {
            // The archer can only capture from a distance of two
            CharacterType.Archer => distance == 2 ? 1 : 0,
            // The assassin immediately captures the leader if he is adjacent to it
            CharacterType.Assassin => distance == 1 ? s_leader_required_capture_value : 0,
            // The cub never takes part to the capture
            CharacterType.Cub => 0,
            // By default, a character count as 1 and must be adjacent to be taken into account
            _ => distance == 1 ? 1 : 0,
        };
    }

    /// <summary>
    /// Returns true if the leader with the specified color is captured.
    /// A leader is captured when enough opponent are placed in a position where they
    /// can take part to the capture. The required position depend of the character
    /// </summary>
    public static bool IsLeaderCaptured(Game game, GameHistory history, TeamColor leaderColor)
    {
        Cell leaderCell = BoardQuery.GetLeaderCell(game.Board, leaderColor);
        // A leader is captured when its capture value reaches "s_leader_capture_value".
        // By default, this value increase when an enemy character is adjacent to the leader
        // but some characters have specific captures rules implemented through "GetCaptureValue"
        int totalCaptureValue = 0;
        // We check every direction around the leader in a straight line to verify if there
        // are  any character susceptible to increase the capture value
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            // Since we always deal with the next cell in a direction, the capture distance is initialized to 1.
            byte distance = 1;
            Cell? nextCell = leaderCell;
            do
            {
                if (nextCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell))
                {
                    nextCell = adjacentCell;
                }
                else
                {
                    nextCell = null;
                }
                totalCaptureValue += GetCaptureValue(nextCell, distance, leaderColor);
                // If the required capture value is reached, it means that the leader is captured
                if (totalCaptureValue >= s_leader_required_capture_value)
                {
                    return true;
                }
                distance++;
            } while (distance <= s_max_capture_distance && nextCell is not null);
        }
        // If we get here, it means that the required capture value wasn't reached
        return false;
    }

    /// <summary>
    /// Returns true if the leader with the specified color is surrounded.
    /// A leader is surrounded when every of its adjacent tiles are occupied
    /// </summary>
    public static bool IsLeaderSurrounded(Game game, TeamColor leaderColor)
    {
        Cell leaderCell = BoardQuery.GetLeaderCell(game.Board, leaderColor);
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            // If at least one cell around the leader is empty, we can exit 
            // immediately since we can be sure that it is not surrounded
            if (leaderCell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && adjacentCell.Character is null)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// During a barrage detection, every cell occupied with an opponent character or empty
    /// is considered as a "non-barrage" cell, used to check if cells are isolated by a barrage
    /// </summary>
    private static List<Cell> GetNonBarrageCells(Board board, TeamColor playerColor)
    {
        List<Cell> nonBarrageCells = new List<Cell>();
        foreach (Cell[] columnCells in board.Cells)
        {
            foreach (Cell cell in columnCells)
            {
                // We add every cell with no piece or with an opponent's piece
                if (cell.Character is null || cell.Character.Color != playerColor)
                {
                    nonBarrageCells.Add(cell);
                }
            }
        }
        return nonBarrageCells;
    }

    /// <summary>
    /// Recursively collects all cells connected to "cell" within nonConnectecCells removing 
    /// each visited cell from the list to prevent revisits across recursive calls
    /// </summary>
    private static void GatherConnectedCells(Board board, Cell cell, List<Cell> nonConnectecCells, List<Cell> connectedCells)
    {
        // First, we add the cell to the connected list
        connectedCells.Add(cell);
        nonConnectecCells.Remove(cell);
        // Then, we go through its adjacent cells to look for other cells to connect
        foreach (Direction direction in DirectionExtension.AllDirections)
        {
            if (cell.AdjacentCells.TryGetValue(direction, out Cell? adjacentCell) && nonConnectecCells.Contains(adjacentCell))
            {
                GatherConnectedCells(board, adjacentCell, nonConnectecCells, connectedCells);
            }
        }
    }

    /// <summary>
    /// Returns the group of cells connected to "cell"
    /// </summary>
    private static List<Cell> GetConnectedCells(Board board, Cell cell, List<Cell> nonConnectedCells)
    {
        List<Cell> connectedCells = new List<Cell>();
        GatherConnectedCells(board, cell, nonConnectedCells, connectedCells);
        return connectedCells;
    }

    /// <summary>
    /// Returns true if at least 2 groups containing an empty cell exists
    /// </summary>
    private static bool HasIsolatedGroup(List<List<Cell>> connectedCellsGroups)
    {
        // There cannot be an isolated group if there's only one group
        if (connectedCellsGroups.Count < 2)
        {
            return false;
        }

        // We count how many groups contain at least one empty cell.
        // As soon as two such groups are found, we exit the function
        int groupCount = 0;
        foreach (List<Cell> cellsGroup in connectedCellsGroups)
        {
            foreach (Cell cell in cellsGroup)
            {
                if (cell.Character is null)
                {
                    groupCount++;
                    if (groupCount > 1)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Detects whether the player linked to playerColor has created a barrage.
    /// A barrage is a chain of 4 allied characters placed to isolate at
    /// least one empty cell from the rest of the board.
    /// </summary>
    public static bool IsBarrageDetected(Game game, TeamColor playerColor)
    {
        List<Cell> playerCharacterCells = BoardQuery.FindCellsWithMatchingCharacter(game.Board, playerColor, null);
        // First we check if the player has 4 characters or more
        if (playerCharacterCells.Count < 4)
        {
            return false;
        }

        // Then we check if 4 or more of the player's characters are chained
        bool hasChainOfFour = false;
        while (!hasChainOfFour && playerCharacterCells.Count > 0)
        {
            // GetConnectedCells mutates "playerCharacterCells" to remove found connected cells 
            if (GetConnectedCells(game.Board, playerCharacterCells[0], playerCharacterCells).Count >= 4)
            {
                hasChainOfFour = true;
            }
        }
        if (!hasChainOfFour)
        {
            return false;
        }

        // Then we get every cell able to be isolated by the barrage
        List<Cell> nonBarrageCells = GetNonBarrageCells(game.Board, playerColor);
        List<List<Cell>> connectedCellsGroups = new List<List<Cell>>();
        while (nonBarrageCells.Count > 0)
        {
            // We group connected cells together within connectedCellsGroups
            connectedCellsGroups.Add(GetConnectedCells(game.Board, nonBarrageCells[0], nonBarrageCells));
            // If an isolated group has been found, we can exit the function immediately
            if (HasIsolatedGroup(connectedCellsGroups))
            {
                return true;
            }
        }

        // We only get here when no isolated group has been found, which means "no barrage"
        return false;
    }
}