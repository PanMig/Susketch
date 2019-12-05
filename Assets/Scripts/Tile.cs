using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TileMapLogic;
using static AuthoringTool;

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
        X = posX;
        Y = posY;
        this.parent = parent;
        gameObj = GameObject.Instantiate(prefab, parent);
        gameObj.name = "X: " + posX + " Y: " + posY;
        image = gameObj.GetComponent<Image>();
        this.envTileID = envTileID;
        this.decID = decID;
    }

    public void PaintTile(TileThemes tileTheme, TileMap tileMap)
    {
        image.sprite = tileTheme.sprite;
        envTileID = tileTheme.envTileID;
        tileMap.SetTileMapTile(this);
    }

    public void SetTile(Tile tile)
    {
        image.sprite = tile.image.sprite;
        envTileID = tile.envTileID;
        decID = tile.decID;
    }

    public void PaintTile(Color color)
    {
        image.color = color;
    }

    public void Highlight(int team)
    {
        if(team == 0)
        {
            GameObject decorationObj = GameObject.Instantiate(Brush.Instance.highlightPrefabRed, gameObj.transform);
            ResizeDecoration(decorationObj, 1.0f);
        }
        else
        {
            GameObject decorationObj = GameObject.Instantiate(Brush.Instance.highlightPrefabBlue, gameObj.transform);
            ResizeDecoration(decorationObj, 1.0f);
        }

    }

    public void Unhighlight()
    {
        for (int i = 0; i < gameObj.transform.childCount; i++)
        {
            if(gameObj.transform.GetChild(i).gameObject.name == Brush.Instance.highlightPrefabRed.gameObject.name + "(Clone)" ||
               gameObj.transform.GetChild(i).gameObject.name == Brush.Instance.highlightPrefabBlue.gameObject.name + "(Clone)")
            {
                GameObject.Destroy(gameObj.transform.GetChild(i).gameObject);
            }
        }
    }

    public void PaintDecoration(Decoration dec, TileMap tileMap)
    {
        // case where tile is not decorated.
        if (dec.prefab != null && gameObj.transform.childCount == 0)
        {
            GameObject decorationObj = GameObject.Instantiate(dec.prefab, gameObj.transform);
            ResizeDecoration(decorationObj, 0.6f);
            decID = dec.decorationID;
            tileMap.SetTileMapTile(this);
        }
        // case where we change the tile sprite, no reason for instatiation.
        else if (dec.prefab != null && gameObj.transform.childCount > 0)
        {
            gameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = dec.sprite;
            decID = dec.decorationID;
            tileMap.SetTileMapTile(this);
        }
        // this means we are using the eraser.
        else if(dec.prefab == null && gameObj.transform.childCount > 0)
        {
            RemoveDecoration(tileMapMain);
        }
    }

    public void RemoveDecoration(TileMap tileMap)
    {
        if (gameObj.transform.childCount > 0)
        {
            GameObject.Destroy(this.gameObj.transform.GetChild(0).gameObject);
            decID = TileEnums.Decorations.empty;
            tileMap.SetTileMapTile(this);
        }
    }

    private void ResizeDecoration(GameObject decoration, float removePercent)
    {
        decoration.GetComponent<RectTransform>().sizeDelta =
            new Vector2(gameObj.GetComponent<RectTransform>().sizeDelta.x * removePercent, gameObj.GetComponent<RectTransform>().sizeDelta.y * removePercent);
    }
}
