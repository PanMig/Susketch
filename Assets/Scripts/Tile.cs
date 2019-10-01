using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public struct Tile
{
    public GameObject gameobject;
    public Image image;
    public Transform parent;
    public int X;
    public int Y;
    public int tileID;

    public Tile(GameObject prefab, Transform parent, int tileID, int posX, int posY)
    {
        this.X = posX;
        this.Y = posY;
        this.parent = parent;
        this.gameobject = GameObject.Instantiate(prefab, parent);
        gameobject.name = "X: " + posX + " Y: " + posY;
        image = gameobject.GetComponent<Image>();
        this.tileID = tileID;
    }

    public void SetTile(TileThemes tileTheme)
    {
        image.sprite = tileTheme.sprite;
        tileID = tileTheme.tileID;
    }
}
