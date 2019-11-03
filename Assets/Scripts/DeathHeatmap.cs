using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TileMapLogic;

//TODO : there is a lot of repeat code from TileMapview needs fix
public class DeathHeatmap : MonoBehaviour
{
    private Tile[,] heatmap = new Tile[4, 4];
    private RectTransform gridRect;
    private GridLayoutGroup gridLayoutGroup;
    public GameObject prefab;


    // Start is called before the first frame update
    void Start()
    {
        InitView(); 
    }

    public void InitView()
    {
        gridRect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        CreateHeatmap();
        gameObject.SetActive(false);
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

    public void SetGridLayoutGroup(int rows, int cols)
    {
        if (gridLayoutGroup != null)
        {
            float _desiredWidth = rows * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float _desiredheight = cols * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
        }
    }

    public void DisplayDeathHeatmap(float[,] values)
    {
        SetGridLayoutGroup(4, 4);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var normalized = values[i, j] * 10.0f;
                heatmap[i, j].SetTile(new Color(normalized, 0 , 0, 1.0f));
            }
        }
        gameObject.SetActive(true);
    }
}
