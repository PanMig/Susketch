using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMapView : MonoBehaviour
{
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
            float _desiredWidth = TileMap.columns * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float _desiredheight = TileMap.rows * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
        }
    }
}
