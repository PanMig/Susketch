using UnityEngine;
using System.Collections;
using Boo.Lang;

public class Region
{
    public Tile[,] tileSet = new Tile[5,5];

    public Region(Tile[,] tileSet)
    {
        this.tileSet = tileSet;
    }

    public List<Tile> GetPerimetricTiles()
    {
        var tiles = new List<Tile>();

        //row tiles
        for (int j = 0; j < 5; j++)
        {
            tiles.Add(tileSet[0, j]);
        }
        //row tiles
        for (int j = 0; j < 5; j++)
        {
            tiles.Add(tileSet[4, j]);
        }
        //column
        for (int i = 0; i < 5; i++)
        {
            tiles.Add(tileSet[i, 0]);
        }
        for (int i = 0; i < 5; i++)
        {
            tiles.Add(tileSet[i, 4]);
        }

        return tiles;
    }
}
