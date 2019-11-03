using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathHeatmap : MonoBehaviour
{
    private Tile[,] heatmap = new Tile[4, 4];
    private RectTransform gridRect;
    private GridLayoutGroup gridLayoutGroup;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        gridRect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        CreateHeatmap();
        gameObject.SetActive(false);
    }

    private void SetGridLayoutGroup()
    {
        if (gridLayoutGroup != null)
        {
            float _desiredWidth = 4 * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float _desiredheight = 4 * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
        }
    }

    private void CreateHeatmap()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                heatmap[i, j] = new Tile(prefab, this.transform, 0, 0, 0, 0);
            }
        }
    }
    
    public void DisplayDeathHeatmap(float[,] values)
    {
        SetGridLayoutGroup();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                heatmap[i, j].SetTile(new Color(values[i, j] * 10, 0 , 0, 255.0f));
            }
        }
        gameObject.SetActive(true);
    }
}
