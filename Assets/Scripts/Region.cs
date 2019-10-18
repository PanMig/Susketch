using UnityEngine;
using System.Collections;

public class Region
{
    public Tile[,] tileSet = new Tile[5,5];

    public Region(Tile[,] tileSet)
    {
        this.tileSet = tileSet;
    }
}
