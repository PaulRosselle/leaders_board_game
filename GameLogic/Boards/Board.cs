using System;
using System.Collections.Generic;
using LeadersBoardGame.GameLogic.Pieces;
using LeadersBoardGame.GameLogic.Players;

namespace LeadersBoardGame.GameLogic.Boards;

public class Board
{
    public Tile[][] Tiles { get; }

    public Board()
    {
        byte columnsCount = BoardConfig.GetColumnsCount();
        Tiles = new Tile[columnsCount][];
        for (byte x = 0; x < columnsCount; x++)
        {
            byte rowsCount = BoardConfig.GetRowsCount(x);
            Tiles[x] = new Tile[rowsCount];
            for (byte y = 0; y < rowsCount; y++)
            {
                Tiles[x][y] = new Tile(new Position(x, y));
            }
        }
    }

    public Board(Board refBoard)
    {
        // Since every board dimensions are given through the same config class, we can
        // use it to initiate the array then populate it with copies of the refBoard tiles
        byte columnsCount = BoardConfig.GetColumnsCount();
        Tiles = new Tile[columnsCount][];
        for (byte x = 0; x < columnsCount; x++)
        {
            byte rowsCount = BoardConfig.GetRowsCount(x);
            Tiles[x] = new Tile[rowsCount];
            for (byte y = 0; y < rowsCount; y++)
            {
                Tiles[x][y] = new Tile(refBoard.Tiles[x][y]);
            }
        }
    }

    /// <summary>
    /// Returns the adjacent tile in a given direction. If there is none, returns null instead
    /// </summary>
    public Tile? FindAdjacentTile(int originX, int originY, Direction direction)
    {
        // Since in most cases only one of the coordinate value changes, we initialize them with the origin value
        int nextXPos = originX;
        int nextYPos = originY;
        if (direction.IsSameColumn()) {
            // If we're looking for a tile on the same column, we apply to y the next value in the given direction
            if (direction.IsTop()) {
                nextYPos--;
            } else {
                nextYPos++;
            }
        } else {
            // Knowing the tiles aren't on the same column at this point, we apply to x the next value in the given direction
            if (direction.IsLeft()) {
                nextXPos--;
            } else {
                nextXPos++;
            }
            // Given that C# integer division truncates the value, "Math.Floor" is unnecessary to get the center tile idx
            int centerColumnIdx = Tiles.Length / 2;
            // Tiles have up to 6 adjacent tiles, meaning that we can see them as hexagons.
            // Foremost, each column is offset from its neighbors since the board itself is hexagonally shaped.
            // For this reason, we must offset the y value for TopLeft/TopRight directions to take this into account
            bool directionIsTowardExterior = (originX == centerColumnIdx) ||
                    (direction.IsLeft() && originX < centerColumnIdx) ||
                    (!direction.IsLeft() && originX > centerColumnIdx);
            int yOffset = directionIsTowardExterior ? -1 : 0;
            // The BottomLeft/BottomRight tiles are always placed immediately below the TopLeft/TopRight adjacentTiles,
            // meaning we can just add 1 to the offset to get their index on the board
            if (!direction.IsTop()) {
                yOffset++;
            }
            nextYPos += yOffset;
        }
        // If the adjacent tile index is out of bounds, we return null
        if (nextXPos < 0 || nextXPos > Tiles.GetUpperBound(0) ||
                nextYPos < 0 || nextYPos > Tiles.GetUpperBound(1)) {
            return null;
        }
        return Tiles[nextXPos][nextYPos];
    }
    
    /// <summary>
    /// Returns the adjacent tile in a given direction. If there is none, returns null instead
    /// </summary>
    public Tile? FindAdjacentTile(Position originPos, Direction direction)
    {
        return FindAdjacentTile(originPos.X, originPos.Y, direction);
    }

    /// <summary>
    /// Return tiles with a non-null piece matching the given parameters
    /// </summary>
    public List<Tile> FindTilesWithMatchingPiece(PlayerColor? pieceColor, PieceKind? pieceKind)
    {
        List<Tile> tilesWithMatchingPiece = new List<Tile>();
        foreach (Tile[] columnTiles in Tiles)
        {
            foreach (Tile tile in columnTiles)
            {
                if (tile.Piece is not null && 
                    (pieceColor is null || tile.Piece.Color == pieceColor) && 
                    (pieceKind is null || tile.Piece.Kind == pieceKind))
                {
                    tilesWithMatchingPiece.Add(tile);
                }
            }
        }
        return tilesWithMatchingPiece;
    }

    /// <summary>
    /// Return tiles with a non-null piece matching the color and kind of the one given in parameter
    /// </summary>
    public List<Tile> FindTilesWithMatchingPiece(Piece piece)
    {
        return FindTilesWithMatchingPiece(piece.Color, piece.Kind);
    }

    /// <summary>
    /// Returns the first tile containing a piece with a matching Id.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the piece is not on the board during the call. This indicates a programming error.
    /// </exception>
    public Tile GetPieceTileById(int pieceId)
    {
        foreach (Tile[] columnTiles in Tiles)
        {
            foreach (Tile tile in columnTiles)
            {
                if (tile.Piece is not null && tile.Piece.Id == pieceId)
                {
                    return tile;
                }
            }
        }
        throw new InvalidOperationException($"No piece found on the board with id {pieceId}");
    }
}