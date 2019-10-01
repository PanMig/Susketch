using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileMap : MonoBehaviour, IPointerClickHandler
{
    public GameObject groundTile;

    public int rows = 20;
    public int columns = 20;
    private static Tile[,] tileMap;

    private RectTransform gridRect;
    private GridLayoutGroup gridLayoutGroup;
    public GridLayout grid;

    [SerializeField] private TileThemes[] tileThemes;
    private int currTileID;

    public int cellSize;
    public int spacing;

    public void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        grid = GetComponent<GridLayout>();

        // set the panel that holds the grid
        SetGridLayoutGroup();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Vector2 v;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main, out v);

            //Debug.Log(v);
        }
    }

    public void SetGrid()
    {
        float _desiredWidth = columns * (cellSize + spacing);
        Debug.Log(_desiredWidth);
        float _desiredheight = rows * (cellSize + spacing);
        Debug.Log(_desiredheight);
        gridRect.sizeDelta = new Vector2(_desiredWidth, _desiredheight);
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

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                tileTheme = tileThemes[Random.Range(0, 1)];
                tileMap[row, col] = new Tile(tileTheme.prefab , gridRect.transform, tileTheme.tileID, row, col);
            }
        }

        StartCoroutine(CTest());
    }

    private IEnumerator CTest()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(tileMap[0, 0].image.rectTransform.localPosition.x);
        Debug.Log(tileMap[0, 0].image.rectTransform.localPosition.y);
    }

    public Tile GetTileWithIndex(int x, int y)
    {
        return tileMap[x, y];
    }

    public Tile GetTileWithPosition(int x, int y)
    {
        return tileMap[x, y];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.transform.localPosition);
        var x = eventData.pointerCurrentRaycast.gameObject.transform.localPosition.x;
        var y = eventData.pointerCurrentRaycast.gameObject.transform.localPosition.y;

        var index_x = (int) Mathf.Floor(x/52);
        var index_y = (int) Mathf.Floor(y/45);

        Tile tile = GetTileWithIndex(index_x, index_y);
        //tile.SetTile(tileThemes[1]);
    }
}
