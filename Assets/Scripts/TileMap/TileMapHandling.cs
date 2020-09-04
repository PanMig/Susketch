using System.Collections.Generic;
using TileMapLogic;

namespace TileMapLogic
{
    public struct OrientationMatrix
    {
        private char[,] _orientation;

        public OrientationMatrix(char[,] orientation)
        {
            this._orientation = orientation;
        }

        public char[,] GetOrientation()
        {
            return _orientation;
        }
    }

    public partial class TileMap
    {
        public void FormatTileOrientation(int x, int y, TileEnums.EnviromentTiles tileType)
        {
            Tile tile = GetTileWithIndex(x, y);
            if (tile.decID == TileEnums.Decorations.stairs)
            {
                SetStairsOrientationTile(tile);
            }
            else if (tile.envTileID != tileType)
            {
                return;
            }

            var neighbours = PathUtils.GetNeighboursToArray(tile, this);
            var orientation = new char[3,3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(i == 1 && j == 1)
                    {
                        orientation[i, j] = 'X';
                        continue;
                    }
                    if (neighbours[i, j] == null)
                    {
                        orientation[i, j] = 'I';
                        continue;
                    }
                    if (neighbours[i, j].envTileID == tileType || neighbours[i, j].decID == TileEnums.Decorations.stairs)
                    {
                        orientation[i, j] = 'P';
                    }
                    else
                    {
                        orientation[i, j] = 'I';
                    }
                }
            }

            //var tileMatrix = new OrientationMatrix(orientation);
            TileOrientations.ruleTiles.TryGetValue(orientation, out var spriteDict);
            
            spriteDict.TryGetValue(orientation, out var tileSprite);

            tile.FormatTileSprite(this, tileSprite);
        }

        public void SetStairsOrientationTile(Tile tile)
        {
            var stairNeighbours = PathUtils.GetNeighboursCross(tile, this);
            if (stairNeighbours.Count >= 1 && stairNeighbours[0] != null)
            {
                if (stairNeighbours[0].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    TileOrientations.stairTiles.TryGetValue(TileOrientations.StairDirection.down, out var decSprite);
                    tile.FormatDecorationSprite(this, decSprite);
                }
            }
            if (stairNeighbours.Count >= 2 &&stairNeighbours[1] != null)
            {
                if (stairNeighbours[1].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    TileOrientations.stairTiles.TryGetValue(TileOrientations.StairDirection.up, out var decSprite);
                    tile.FormatDecorationSprite(this, decSprite);
                }
            }
            if (stairNeighbours.Count >= 3 && stairNeighbours[2] != null)
            {
                if (stairNeighbours[2].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    TileOrientations.stairTiles.TryGetValue(TileOrientations.StairDirection.right, out var decSprite);
                    tile.FormatDecorationSprite(this, decSprite);
                }
            }
            if (stairNeighbours.Count >= 4 && stairNeighbours[3] != null)
            {
                if (stairNeighbours[3].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    TileOrientations.stairTiles.TryGetValue(TileOrientations.StairDirection.left, out var decSprite);
                    tile.FormatDecorationSprite(this, decSprite);
                }
            }
        }
    }
}


public class TileMapComparer : IEqualityComparer<Tile[,]>
{
    public bool Equals(Tile[,] map_x, Tile[,] map_y)
    {
        for (int i = 0; i < TileMap.rows; i++)
        {
            for (int j = 0; j < TileMap.columns; j++)
            {
                if (map_x[i, j].envTileID == map_y[i, j].envTileID && map_x[i, j].decID == map_y[i, j].decID)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int GetHashCode(Tile[,] obj)
    {
        int hash = 17;
        for (int i = 0; i < TileMap.rows; i++)
        {
            for (int j = 0; j < TileMap.columns; j++)
            {
                hash = hash * 31 + (int)obj[i, j].envTileID + (int)obj[i, j].decID;
            }
        }

        return hash;
    }
}
