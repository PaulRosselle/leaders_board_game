namespace LeadersBoardGame.GameLogic.Handlers;

using System;
using LeadersBoardGame.GameLogic.Boards;
using LeadersBoardGame.GameLogic.Players;
using LeadersBoardGame.GameLogic.Pieces;
using System.Collections.Generic;

public static class GameEndConditionChecker
{
    // Currently the maximum distance were a character can be taken into 
    // account is 2 tiles appart from the leader (Archer passive effect)
    private static byte s_max_capture_distance = 2;
    private static byte s_leader_required_capture_value = 2;

    /// <summary>
    /// During a barrage detection, every tile occupied with an opponent piece or empty
    /// is considered as a "non-barrage" tile, used to check if tiles are isolated by a barrage
    /// </summary>
    private static List<Tile> GetNonBarrageTiles(PlayerColor playerColor, Board board)
    {
        List<Tile> nonBarrageTiles = new List<Tile>();
        foreach (Tile[] columnTiles in board.Tiles)
        {
            foreach (Tile tile in columnTiles)
            {
                // We add every tile with no piece or with an opponent's piece
                if (tile.Piece is null || tile.Piece.Color != playerColor)
                {
                    nonBarrageTiles.Add(tile);
                }
            }
        }
        return nonBarrageTiles;
    }

    /// <summary>
    /// Recursively collects all tiles connected to tile within nonConnectecTiles removing 
    /// each visited tile from the list to prevent revisits across recursive calls
    /// </summary>
    private static void GatherConnectedTiles(Tile tile, List<Tile> nonConnectecTiles, Board board, List<Tile> connectedTiles)
    {
        // First, we add the tile to the connected list
        connectedTiles.Add(tile);
        nonConnectecTiles.Remove(tile);
        // Then, we go through its adjacent tiles to look for other tiles to connect
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Tile? adjacentTile = board.FindAdjacentTile(tile, direction);
            if (adjacentTile is not null && nonConnectecTiles.Contains(adjacentTile))
            {
                GatherConnectedTiles(adjacentTile, nonConnectecTiles, board, connectedTiles);
            }
        }
    }

    /// <summary>
    /// Returns the group of tiles connected to tile
    /// </summary>
    private static List<Tile> GetConnectedTiles(Tile tile, List<Tile> nonConnectecTiles, Board board)
    {
        List<Tile> connectedTiles = new List<Tile>();
        GatherConnectedTiles(tile, nonConnectecTiles, board, connectedTiles);
        return connectedTiles;
    }

    /// <summary>
    /// Returns true if at least 2 groups containing an empty tile exists
    /// </summary>
    private static bool HasIsolatedGroup(List<List<Tile>> connectedTilesGroups)
    {
        // There cannot be an isolated group if there's only one group
        if (connectedTilesGroups.Count < 2)
        {
            return false;
        }

        // We count how many groups contain at least one empty tile.
        // As soon as two such groups are found, we exit the function
        int groupCount = 0;
        foreach (List<Tile> tilesGroup in connectedTilesGroups)
        {
            foreach (Tile tile in tilesGroup)
            {
                if (tile.Piece is null)
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
    /// least one empty tile from the rest of the board.
    /// </summary>
    public static bool IsBarrageDetected(PlayerColor playerColor, Board board)
    {
        List<Tile> playerPieceTiles = board.FindTilesWithMatchingPiece(playerColor, null);
        // First we check if the player has 4 characters or more
        if (playerPieceTiles.Count < 4)
        {
            return false;
        }

        // Then we check if 4 or more of the player's pieces are chained
        bool hasChainOfFour = false;
        while (playerPieceTiles.Count > 0)
        {
            if (GetConnectedTiles(playerPieceTiles[0], playerPieceTiles, board).Count >= 4)
            {
                hasChainOfFour = true;
                break;
            }
        }
        if (!hasChainOfFour)
        {
            return false;
        }

        // Then we get every tile able to be isolated by the barrage
        List<Tile> nonBarrageTiles = GetNonBarrageTiles(playerColor, board);
        List<List<Tile>> connectedTilesGroups = new List<List<Tile>>();
        while (nonBarrageTiles.Count > 0)
        {
            // We group connected tiles together within connectedTilesGroups
            connectedTilesGroups.Add(GetConnectedTiles(nonBarrageTiles[0], nonBarrageTiles, board));
            // If an isolated group has been found, we can exit the function immediately
            if (HasIsolatedGroup(connectedTilesGroups))
            {
                return true;
            }
        }

        // We only get here when no isolated group has been found, which means "no barrage"
        return false;
    }

    /// <summary>
    /// Returns the capture value of the piece within the parameter tile based on the distance.
    /// </summary>
    private static int GetCaptureValue(Tile? tile, byte distance, PlayerColor leaderColor)
    {
        // A null or empty tile doesn't have a capture value and only opponent pieces are taken into account
        if (tile is null || tile.Piece is null || tile.Piece.Color == leaderColor)
        {
            return 0;
        }

        return tile.Piece.Kind switch
        {
            // The archer can only capture from a distance of two
            PieceKind.Archer => distance == 2 ? 1 : 0,
            // The assassin immediately captures the leader if he is adjacent to it
            PieceKind.Assassin => distance == 1 ? s_leader_required_capture_value : 0,
            // The cub never takes part to the capture
            PieceKind.Cub => 0,
            // By default, a character count as 1 and must be adjacent to be taken into account
            _ => distance == 1 ? 1 : 0,
        };
    }

    /// <summary>
    /// Returns true if the leader with the specified color is captured.
    /// A leader is captured when enough opponent are placed in a position where they
    /// can take part to the capture. The required position depend of the character
    /// </summary>
    public static bool IsLeaderCaptured(PlayerColor leaderColor, Board board)
    {
        Tile leaderTile = board.GetLeaderTile(leaderColor);
        // A leader is captured when its capture value reaches "s_leader_capture_value".
        // By default, this value increase when an enemy character is adjacent to the leader
        // but some characters have specific captures rules implemented through "GetCaptureValue"
        int totalCaptureValue = 0;
        // We check every direction around the leader in a straight line to verify if there
        // are  any character susceptible to increase the capture value
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            // Since we always deal with the next tile in a direction, the capture distance is initialized to 1.
            byte distance = 1;
            Tile? nextTile = leaderTile;
            do
            {
                nextTile = board.FindAdjacentTile(nextTile, direction);
                totalCaptureValue += GetCaptureValue(nextTile, distance, leaderColor);
                // If the required capture value is reached, it means that the leader is captured
                if (totalCaptureValue >= s_leader_required_capture_value)
                {
                    return true;
                }
                distance++;
            } while (distance <= s_max_capture_distance && nextTile is not null);
        }
        // If we get here, it means that the required capture value wasn't reached
        return false;
    }

    /// <summary>
    /// Returns true if the leader with the specified color is surrounded.
    /// A leader is surrounded when every of its adjacent tiles are occupied
    /// </summary>
    public static bool IsLeaderSurrounded(PlayerColor leaderColor, Board board)
    {
        Tile leaderTile = board.GetLeaderTile(leaderColor);
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Tile? adjacentTile = board.FindAdjacentTile(leaderTile, direction);
            // If at least one tile around the leader is empty, we can exit 
            // immediately since we can be sure that it is not surrounded
            if (adjacentTile is not null && adjacentTile.Piece is null)
            {
                return false;
            }
        }
        return true;
    }
}