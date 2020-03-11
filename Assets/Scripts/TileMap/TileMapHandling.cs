using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMapLogic
{
    public partial class TileMap
    {
        public enum orientation {standalone, top, bottom, middle};

        public void FormatTileOrientation(int x, int y, HashSet<Tile> orientedTiles, orientation Orientation)
        {
            var firstFloor = TileEnums.EnviromentTiles.level_1;
            Tile tile = GetTileWithIndex(x, y);
            if(tile.envTileID != firstFloor || orientedTiles.Contains(tile))
            {
                return;
            }
            var neighbours = PathUtils.GetNeighboursCross(tile, this);
            //Standalone
            if(neighbours[1].envTileID != firstFloor && neighbours[0].envTileID != firstFloor)
            {
                // Set Tile Standalone
                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
            }
            //right
            else if (neighbours[3].envTileID == firstFloor && neighbours[2].envTileID != firstFloor)
            {
                Debug.Log("right");
                orientedTiles.Add(tile);
                switch (Orientation)
                {
                    case orientation.standalone:
                        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[2]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y , orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X - 1, tile.Y , orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.middle);
                        break;
                    case orientation.top:
                        tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[2]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.top);
                        break;
                    case orientation.bottom:
                        tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[2]);
                        tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[2]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.bottom);
                        break;
                    case orientation.middle:
                        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[2]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.middle);
                        break;
                }
            }
            else if (neighbours[2].envTileID == firstFloor && neighbours[3].envTileID != firstFloor)
            {
                // Set Tile top
                Debug.Log("left");
                orientedTiles.Add(tile);
                switch (Orientation)
                {
                    case orientation.standalone:
                        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[1]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.middle);
                        break;
                    case orientation.top:
                        tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[1]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.top);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.top);
                        break;
                    case orientation.bottom:
                        tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[1]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.bottom);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.bottom);
                        break;
                    case orientation.middle:
                        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[1]);
                        FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
                        FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.middle);
                        break;
                }
            }
            //Top tile
            else if(neighbours[0].envTileID == firstFloor && neighbours[1].envTileID != firstFloor)
            {
                // Set Tile top
                Debug.Log("Top tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.top);
                FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.top);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.top);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.top);
                FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.top);
            }
            //Bottom tile
            else if (neighbours[1].envTileID == firstFloor && neighbours[0].envTileID != firstFloor)
            {
                // Set Tile bottom
                Debug.Log("Bottom tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.bottom);
                FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.bottom);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.bottom);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.bottom);
                FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.bottom);
            }
            //Middle tile
            else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID == firstFloor)
            {
                // Set Tile middle
                Debug.Log("middle tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
                FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
                FormatTileOrientation(tile.X + 1, tile.Y - 1, orientedTiles, orientation.middle);
            }
            else if (neighbours[2].envTileID != firstFloor && neighbours[3].envTileID != firstFloor)
            {
                // Set Tile Standalone
                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[3]);
            }
        }

        private void RecursiveOrientationCall(int x , int y)
        {
            
        }
    }
}
