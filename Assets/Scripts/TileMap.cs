using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


// TODO : Separate model from view
public class TileMap : MonoBehaviour
{
    public static int rows = 20;
    public static int columns = 20;
    private static Tile[,] tileMap;
    private static Region[,] regions = new Region[4,4];
    private readonly int CELL_PER_REGION = 5;
    private readonly int REGIONS = 16;

    private RectTransform gridRect;
    private GridLayoutGroup gridLayoutGroup;


    public void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        // set the panel that holds the grid
        SetGridLayoutGroup();
    }

    public void SetGridLayoutGroup()
    {
        if (gridLayoutGroup != null)
        {
            float _desiredWidth = columns * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float _desiredheight = rows * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
        }
    }

    public void InitTileMap()
    {
        tileMap = new Tile[rows, columns];
        TileThemes tileTheme;
        Decoration dec;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0,1)];
                dec = Brush.Instance.decorations[0];
                tileMap[row, col] = new Tile(tileTheme.prefab , gridRect.transform, tileTheme.envTileID, dec.decorationID ,row, col);
            }
        }
    }

    public void InitRegions()
    {
        int stepX = 0, stepY = 0;
        for (int i = 0; i < 4; i++)
        {
            stepX = i * 5;
            for (int j = 0; j < 4; j++)
            {
                stepY = j * 5;
                regions[i,j] = new Region(FillRegion(stepX, stepY));
            }
        }
    }

    private Tile[,] FillRegion(int stepX, int stepY)
    {
        Tile[,] tileSet = new Tile[5, 5];
        for (int row = 0; row < CELL_PER_REGION; row++)
        {
            for (int col = 0; col < CELL_PER_REGION; col++)
            {
                tileSet[row, col] = tileMap[row+stepX, col+stepY];
            }
        }
        return tileSet;
    }

    public void PaintRegion(int row, int column, int brushIndex)
    {
        for (int i = 0; i < CELL_PER_REGION; i++)
        {
            for (int j = 0; j < CELL_PER_REGION; j++)
            {
                regions[row,column].tileSet[i, j].SetTile(Brush.Instance.brushThemes[brushIndex]);
            }
        }
    }

    public void PaintRegion(int row, int column, Color color)
    {
        for (int i = 0; i < CELL_PER_REGION; i++)
        {
            for (int j = 0; j < CELL_PER_REGION; j++)
            {
                regions[row, column].tileSet[i, j].SetTile(color);
            }
        }
    }

    public void FillTileMap()
    {
        TileThemes tileTheme;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0, 3)];
                tileMap[row, col].SetTile(tileTheme);
            }
        }
    }

    public static Tile GetTileWithIndex(int row, int col)
    {
        return tileMap[row, col];
    }

    public static void SetTileMapTile(int row, int col, Tile tile)
    {
        tileMap[row, col] = tile;
    }

    public string[,] GetTileMapToString()
    {
        string[,] stringMap = new string[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if(tileMap[row,col].envTileID == TileEnums.EnviromentTiles.ground)
                {
                    stringMap[row, col] = "0";
                }
                else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    stringMap[row, col] = "1";
                }
                else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_2)
                {
                    stringMap[row, col] = "2";
                }
                if(tileMap[row, col].decID == TileEnums.Decorations.healthPack)
                {
                    stringMap[row, col] += "H";
                }
                if (tileMap[row, col].decID == TileEnums.Decorations.damageBoost)
                {
                    stringMap[row, col] += "D";
                }
                if (tileMap[row, col].decID == TileEnums.Decorations.armorVest)
                {
                    stringMap[row, col] += "A";
                }
                if (tileMap[row, col].decID == TileEnums.Decorations.stairs)
                {
                    stringMap[row, col] += "S";
                }
            }
        }
        return stringMap;
    }
}
