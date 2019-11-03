using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public struct Tile
{
    public GameObject gameObj;
    public Image image;
    public Transform parent;
    public int X;
    public int Y;
    public TileEnums.EnviromentTiles envTileID;
    public TileEnums.Decorations decID;

    public Tile(GameObject prefab, Transform parent, TileEnums.EnviromentTiles envTileID, TileEnums.Decorations decID ,int posX, int posY)
    {
        this.X = posX;
        this.Y = posY;
        this.parent = parent;
        this.gameObj = GameObject.Instantiate(prefab, parent);
        gameObj.name = "X: " + posX + " Y: " + posY;
        image = gameObj.GetComponent<Image>();
        this.envTileID = envTileID;
        this.decID = decID;
    }

    public void SetTile(TileThemes tileTheme)
    {
        image.sprite = tileTheme.sprite;
        envTileID = tileTheme.envTileID;
        TileMap.SetTileMapTile(X, Y, this);
    }

    public void SetTile(Color color)
    {
        image.color = color;
    }

    public void Highlight()
    {
        GameObject decorationObj = GameObject.Instantiate(Brush.Instance.highlightPrefab, gameObj.transform);
        ResizeDecoration(decorationObj, 1.0f);
    }

    public void Unhighlight()
    {
        for (int i = 0; i < gameObj.transform.childCount; i++)
        {
            if(gameObj.transform.GetChild(i).gameObject.name == Brush.Instance.highlightPrefab.gameObject.name + "(Clone)")
            {
                GameObject.Destroy(gameObj.transform.GetChild(i).gameObject);
            }
        }
    }

    public void SetDecoration(Decoration dec)
    {
        // case where tile is not decorated.
        if (dec.prefab != null && gameObj.transform.childCount == 0)
        {
            GameObject decorationObj = GameObject.Instantiate(dec.prefab, gameObj.transform);
            ResizeDecoration(decorationObj, 0.6f);
            decID = dec.decorationID;
            TileMap.SetTileMapTile(X, Y, this);
        }
        // case where we change the tile sprite, no reason for instatiation.
        else if (dec.prefab != null && gameObj.transform.childCount > 0)
        {
            gameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = dec.sprite;
            decID = dec.decorationID;
            TileMap.SetTileMapTile(X, Y, this);
        }
        // this means we are using the eraser.
        else if(dec.prefab == null && gameObj.transform.childCount > 0)
        {
            GameObject.Destroy(this.gameObj.transform.GetChild(0).gameObject);
            decID = dec.decorationID;
            TileMap.SetTileMapTile(X, Y, this);
        }
    }

    private void ResizeDecoration(GameObject decoration, float removePercent)
    {
        decoration.GetComponent<RectTransform>().sizeDelta =
            new Vector2(gameObj.GetComponent<RectTransform>().sizeDelta.x * removePercent, gameObj.GetComponent<RectTransform>().sizeDelta.y * removePercent);
    }
}
