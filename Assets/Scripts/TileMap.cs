using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// TODO : Separate model from view
public class TileMap : MonoBehaviour
{
    public static int rows = 20;
    public static int columns = 20;
    private static Tile[,] tileMap;

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
                tileTheme = Brush.Instance.brushThemes[Random.Range(0,1)];
                dec = Brush.Instance.decorations[0];
                tileMap[row, col] = new Tile(tileTheme.prefab , gridRect.transform, tileTheme.envTileID, dec.decorationID ,row, col);
            }
        }
        // Player base 1 

        // Player base 2
    }

    public void FillTileMap()
    {
        
        TileThemes tileTheme;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                tileTheme = Brush.Instance.brushThemes[Random.Range(0, 3)];
                tileMap[row, col].SetTile(tileTheme);
            }
        }
    }

    private IEnumerator CTest()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(tileMap[0, 0].image.rectTransform.localPosition.x);
        Debug.Log(tileMap[0, 0].image.rectTransform.localPosition.y);
    }

    public static Tile GetTileWithIndex(int row, int col)
    {
        return tileMap[row, col];
    }

    public static void SetTileMapTile(int row, int col, Tile tile)
    {
        tileMap[row, col] = tile;
    }
}
