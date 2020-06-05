using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TileMapLogic;

public class TileMapView : MonoBehaviour
{
    public RectTransform gridRect;
    public GridLayoutGroup gridLayoutGroup;


    public void Awake()
    {
        InitView();
    }

    public void InitView()
    {
        gridRect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        // set the panel that holds the grid
        SetGridLayoutGroup(TileMap.rows, TileMap.columns);
    }

    public void SetGridLayoutGroup(int rows, int cols)
    {
        if (gridLayoutGroup != null)
        {
            float _desiredWidth = rows * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float _desiredheight = cols * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
        }
    }
}
