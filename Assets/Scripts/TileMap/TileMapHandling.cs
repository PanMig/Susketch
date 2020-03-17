using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMapLogic
{
    public partial class TileMap
    {
        private const TileEnums.EnviromentTiles firstFloor = TileEnums.EnviromentTiles.level_1;
        public enum orientation { standalone, top, bottom, middle };

        public void FormatTileOrientation(int x, int y, HashSet<Tile> orientedTiles, orientation Orientation)
        {

            Tile tile = GetTileWithIndex(x, y);
            if (tile.envTileID != firstFloor || orientedTiles.Contains(tile))
            {
                return;
            }

            // 4 Neighbours in oder D , U, R, L.
            var neighbours = PathUtils.GetNeighboursCross(tile, this);

            //Standalone Tile : all neighbours not first floor.
            if (neighbours[0].envTileID != firstFloor && neighbours[1].envTileID != firstFloor &&
                neighbours[2].envTileID != firstFloor && neighbours[3].envTileID != firstFloor)
            {
                // Set Tile Standalone
                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[3]);
            }
            //Top tile : No first floor on top.
            else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID != firstFloor)
            {
                // Set Tile top
                tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.top);
            }
            //Bottom tile : No first floor below.
            else if (neighbours[1].envTileID == firstFloor && neighbours[0].envTileID != firstFloor)
            {
                // Set Tile top
                tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.bottom);
            }
            //Middle tile
            else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID == firstFloor)
            {
                // Set Tile middle.
                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
                orientedTiles.Add(tile);
                //FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                //FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                //FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
                //FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
            }
            //Right
            if (neighbours[3].envTileID == firstFloor && neighbours[2].envTileID != firstFloor)
            {
                orientedTiles.Add(tile);
                var orientation = GetLeftTileOrientation(tile);
                //top right tile
                if (orientation == orientation.top)
                {
                    tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[2]);
                    var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
                    leftTile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
                    FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
                    FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
                }
                else if (orientation == orientation.bottom)
                {
                    tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[2]);
                    var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
                    leftTile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
                    FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
                    FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
                }
                else if (orientation == orientation.middle)
                {
                    tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[2]);
                    var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
                    leftTile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
                    FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
                    FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
                }
            }
            //left
            //else if (neighbours[2].envTileID == firstFloor && neighbours[3].envTileID != firstFloor)
            //{
            //    orientedTiles.Add(tile);
            //    var orientation = GetRightTileOrientation(tile);
            //    //top right tile
            //    if (orientation == orientation.top)
            //    {
            //        tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[3]);
            //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
            //        rightTile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
            //    }
            //    else if (orientation == orientation.bottom)
            //    {
            //        tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[3]);
            //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
            //        rightTile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
            //    }
            //    else if (orientation == orientation.middle)
            //    {
            //        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[3]);
            //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
            //        rightTile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
            //    }
            //}
        }

        private orientation GetLeftTileOrientation(Tile tile)
        {
            var left = PathUtils.GetNeighbours(tile, this)[3];
            var neighbours = PathUtils.GetNeighbours(left, this);
            if (neighbours[1].envTileID != firstFloor)
            {
                return orientation.top;
            }
            if (neighbours[0].envTileID != firstFloor)
            {
                return orientation.bottom;
            }
            return orientation.middle;
        }

        private orientation GetRightTileOrientation(Tile tile)
        {
            var right = PathUtils.GetNeighbours(tile, this)[2];
            var neighbours = PathUtils.GetNeighbours(right, this);
            if (neighbours[1].envTileID != firstFloor)
            {
                return orientation.top;
            }
            if (neighbours[0].envTileID != firstFloor)
            {
                return orientation.bottom;
            }
            return orientation.middle;
        }
    }
}
