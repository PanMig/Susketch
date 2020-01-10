using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMapLogic
{
    public partial class TileMap
    {
        public void FormatTileOrientation(int x, int y, HashSet<Tile> orientedTiles)
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
                Debug.Log("Standalone");
                tile.FormatTileSprite(this, TileOrientations.Instance.firstFloorOrientations[0]);
            }
            //Top tile
            else if(neighbours[0].envTileID == firstFloor && neighbours[1].envTileID != firstFloor)
            {
                // Set Tile top
                Debug.Log("Top tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.firstFloorOrientations[1]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles);
            }
            //Bottom tile
            else if (neighbours[1].envTileID == firstFloor && neighbours[0].envTileID != firstFloor)
            {
                // Set Tile bottom
                Debug.Log("Bottom tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.firstFloorOrientations[2]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles);
            }
            //Middle tile
            else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID == firstFloor)
            {
                // Set Tile middle
                Debug.Log("middle tile");
                tile.FormatTileSprite(this, TileOrientations.Instance.firstFloorOrientations[3]);
                orientedTiles.Add(tile);
                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles);
                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles);
            }
        }

        private void RecursiveOrientationCall(int x , int y)
        {
            
        }
    }
}
