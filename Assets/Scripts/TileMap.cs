using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public GameObject groundTile;
    public GameObject level1Tile;
    public GameObject level2Tile;

    private const int rows = 20;
    private const int columns = 20;
    private static int[,] tileMap = new int[rows, columns];

    public void InitializeTileMap()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                tileMap[i, j] = 0;
                RenderTile(tileMap[i, j], i, j);
            }
        }
    } 
    
    private void RenderTile(int tileID, int posX, int posY)
    {
        switch (tileID)
        {
            case (int)Tiles.EnviromentTiles.ground:
                GameObject tile = Instantiate(groundTile, new Vector3(posX, posY, 0), Quaternion.identity);
                tile.transform.parent = transform;
                break;
            case (int)Tiles.EnviromentTiles.level_1:
                GameObject tile_1 = Instantiate(groundTile, new Vector3(posX, posY, 0), Quaternion.identity);
                tile_1.transform.parent = transform;
                break;
            case (int)Tiles.EnviromentTiles.level_2:
                GameObject tile_2 = Instantiate(groundTile, new Vector3(posX, posY, 0), Quaternion.identity);
                tile_2.transform.parent = transform;

                break;
            default:
                break;
        }
    }
}
