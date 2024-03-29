﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TileMapLogic;
using static AuthoringTool;

public class Tile
{
    public GameObject gameObj;
    public Image image;
    public int X;
    public int Y;
    public TileEnums.EnviromentTiles envTileID;
    public TileEnums.Decorations decID;
    public Image decorationImage;

    public Tile()
    {

    }

    public Tile(TileEnums.EnviromentTiles envTileID, TileEnums.Decorations decID , int posX, int posY)
    {
        X = posX;
        Y = posY;
        this.envTileID = envTileID;
        this.decID = decID;
        gameObj = null;
        image = null;
        decorationImage = null;
    }

    public void PaintTile(GameObject prefab, TileThemes tileTheme, Decoration dec, Transform parent, float decorationScale)
    {
        // Set view in rect transform.
        gameObj = GameObject.Instantiate(prefab, parent);
        gameObj.name = "X: " + this.X + " Y: " + this.Y;
        image = gameObj.GetComponent<Image>();
        decorationImage = gameObj.transform.GetChild(0).GetComponent<Image>();
        //ResizeDecoration(gameObj.transform.GetChild(0).gameObject, decorationScale);
        SetTheme(tileTheme);
        SetDecoration(dec);
    }

    public void PaintTile(GameObject prefab, Transform parent)
    {
        // Set view in rect transform.
        gameObj = GameObject.Instantiate(prefab, parent);
        gameObj.name = "X: " + this.X + " Y: " + this.Y;
        image = gameObj.GetComponent<Image>();
    }

    public void Render()
    {
        image.sprite = Brush.Instance.brushThemes[(int)envTileID].sprite;
        decorationImage.sprite = Brush.Instance.decorations[(int)decID].sprite;
    }

    public void SetTheme(TileThemes tileTheme)
    {
        image.sprite = tileTheme.sprite;
        envTileID = tileTheme.envTileID;
    }

    public void SetDecoration(Decoration dec)
    {
        decorationImage.sprite = dec.sprite;
        decID = dec.decorationID;
    }

    public void FormatTileSprite(TileMap tileMap, Sprite sprite)
    {
        image.sprite = sprite;
        tileMap.SetTileMapTile(this);
    }

    public void FormatDecorationSprite(TileMap tileMap, Sprite sprite)
    {
        decorationImage.sprite = sprite;
        tileMap.SetTileMapTile(this);
    }

    public void SetTile(Tile tile)
    {
        //only value types are copied.
        image.sprite = tile.image.sprite;
        decorationImage.sprite = tile.decorationImage.sprite;
        envTileID = tile.envTileID;
        decID = tile.decID;
    }

    public Tile ShallowCopy(Tile newTile)
    {
        return (Tile) newTile.MemberwiseClone();
    }

    public void CopyEnvDec(Tile copiedTile)
    {
        this.envTileID = copiedTile.envTileID;
        this.decID = copiedTile.decID;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public Tuple<int, int> GetRegion()
    {
        int step = 5;
        int regions = 4;

        int row_idx = X / step;
        int col_idx = Y / step;
        return new Tuple<int, int>(row_idx, col_idx);
    }

    public void Highlight(int team, int direction)
    {
        if (this.decID == TileEnums.Decorations.healthPack || this.decID == TileEnums.Decorations.armorVest ||
            this.decID == TileEnums.Decorations.damageBoost) return;
        if(team == 0)
        {
            GameObject decorationObj = GameObject.Instantiate(Brush.Instance.highlightPrefabRed, gameObj.transform);
            ResizeDecoration(decorationObj, 0.4f);
            SetDirection(decorationObj, direction);
        }
        else
        {
            GameObject decorationObj = GameObject.Instantiate(Brush.Instance.highlightPrefabBlue, gameObj.transform);
            ResizeDecoration(decorationObj, 0.4f);
            SetDirection(decorationObj, direction);
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

    //public void PaintDecoration(Decoration dec, TileMap tileMap, Transform tileParent)
    //{
    //    // case where tile is not decorated.
    //    if (dec.prefab != null && gameObj.transform.childCount == 0)
    //    {
    //        GameObject decorationObj = GameObject.Instantiate(dec.prefab, gameObj.transform);
    //        ResizeDecoration(decorationObj, 0.6f);
    //        decID = dec.decorationID;
    //        parent = tileParent;
    //        tileMap.SetTileMapTile(this);
    //    }
    //    // case where we change the tile sprite, no reason for instatiation.
    //    else if (dec.prefab != null && gameObj.transform.childCount > 0)
    //    {
    //        gameObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = dec.sprite;
    //        decID = dec.decorationID;
    //        tileMap.SetTileMapTile(this);
    //    }
    //    // this means we are using the eraser.
    //    else if(dec.prefab == null && gameObj.transform.childCount > 0)
    //    {
    //        RemoveDecoration(tileMapMain);
    //    }
    //}

    //public void RemoveDecoration(TileMap tileMap)
    //{
    //    if(gameObj.transform.childCount > 0)
    //    {
    //        GameObject.DestroyImmediate(this.gameObj.transform.GetChild(0).gameObject);
    //    }
    //    decID = TileEnums.Decorations.empty;
    //    tileMap.SetTileMapTile(this);
    //}

    //Resizes the gameobject withing the parent object. Only for decorations.
    public void ResizeDecoration(GameObject decoration, float removePercent)
    {
        decoration.GetComponent<RectTransform>().sizeDelta =
        new Vector2(gameObj.GetComponent<RectTransform>().sizeDelta.x * removePercent, gameObj.GetComponent<RectTransform>().sizeDelta.y * removePercent);
    }

    public void ResizeDecoration(GameObject decoration, float removePercentX, float removePercentY)
    {
        decoration.GetComponent<RectTransform>().sizeDelta =
                new Vector2(gameObj.GetComponent<RectTransform>().sizeDelta.x * (removePercentX), gameObj.GetComponent<RectTransform>().sizeDelta.y * removePercentY);
    }

    private void SetDirection(GameObject decoration, int direction)
    {
        var rect = decoration.GetComponent<RectTransform>();
        switch (direction)
        {
            // Up
            case 0:
                
                break;
            // right
            case 1:
                rect.Rotate(Vector3.forward,90);
                break;
            //down
            case 2:
                rect.Rotate(Vector3.right, 180);
                break;
            case 3:
                rect.Rotate(Vector3.forward, -90);
                break;
        }
    }

}
