using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TileMapLogic;
using static AuthoringTool;

public class TileCursor : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color startcolor;

    public enum CursorType
    {
        tile,
        decoration,
        fill,
    }

    public CursorType currentCursorType;

    #region interface implementations

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentCursorType == CursorType.tile)
        {
            DrawTile(eventData);
        }
        else if (currentCursorType == CursorType.decoration)
        {
            DrawDecoration(eventData);
        }
        else if (currentCursorType == CursorType.fill)
        {
            FillBoundedArea(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentCursorType == CursorType.tile)
        {
            DrawTile(eventData);
        }
        else if (currentCursorType == CursorType.decoration)
        {
            DrawDecoration(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            int index_row, index_col;
            GetIndexFromCoordinates(eventData, out index_row, out index_col);
            var tile = tileMapMain.GetTileWithIndex(index_row, index_col);
            tile.image.color = startcolor;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    #endregion

    private void FillBoundedArea(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            int index_row, index_col;
            GetIndexFromCoordinates(eventData, out index_row, out index_col);
            var oldColor = (int) tileMapMain.GetTileWithIndex(index_row, index_col).envTileID;
            Brush.Instance.FillRegion(index_row, index_col, Brush.Instance.brushThemes[Brush.Instance.currTileBrush], Brush.Instance.brushThemes[oldColor]);
        }

    }

    private static void GetIndexFromCoordinates(PointerEventData eventData, out int index_row, out int index_col)
    {
        // x coordinate corresponds to columns(horizontal) and y corresponds to rows(vertical).
        float x = eventData.pointerCurrentRaycast.gameObject.transform.localPosition.x;
        float y = Mathf.Abs(eventData.pointerCurrentRaycast.gameObject.transform.localPosition.y);

        // Add some extra padding for accurate results.
        index_row = (int)Mathf.Floor(y / 47);
        index_col = (int)Mathf.Floor(x / 52);
    }

    private void DrawTile(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            int index_row, index_col;
            GetIndexFromCoordinates(eventData, out index_row, out index_col);
            SetSingleTile(index_row, index_col);
        }
    }

    private void DrawDecoration(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
                int index_row, index_col;
                GetIndexFromCoordinates(eventData, out index_row, out index_col);
                SetSingleDecoration(index_row, index_col);
        }
    }

    private void SetSingleTile(int index_row, int index_col)
    {
        if (Enumerable.Range(0, 20).Contains(index_row) && Enumerable.Range(0, 20).Contains(index_col))
        {
            // column, row.
            Tile tile = tileMapMain.GetTileWithIndex(index_row, index_col);

            int index = Brush.Instance.currTileBrush;

            tile.PaintTile(Brush.Instance.brushThemes[index], tileMapMain);
            //if(tile.envTileID == TileEnums.EnviromentTiles.level_1)
            //{
            //    tileMapMain.FormatTileOrientation(tile.X, tile.Y, new HashSet<Tile>());
            //}
            //Debug.Log(tile.gameObj.name + "_" + tile.envTileID + " " + tile.decID);
            EventManagerUI.onTileMapEdit?.Invoke();
        }
    }

    private void SetSingleDecoration(int index_row, int index_col)
    {
        if (Enumerable.Range(0, 20).Contains(index_row) && Enumerable.Range(0, 20).Contains(index_col))
        {
            // column, row.
            Tile tile = tileMapMain.GetTileWithIndex(index_row, index_col);

            // zero index is always the empty decoration, that's why we add plus one to current brush index.
            int index = Brush.Instance.currDecBrush;

            tile.PaintDecoration(Brush.Instance.decorations[index], tileMapMain);
            //Debug.Log(tile.gameObj.name + "_" + tile.envTileID + " " + tile.decID);
            EventManagerUI.onTileMapEdit?.Invoke();
        }
    }

    public void SetCursorType(int type)
    {
        if (type == 0) currentCursorType = CursorType.tile;
        else if (type == 1) currentCursorType = CursorType.decoration;
        else if (type == 2) currentCursorType = CursorType.fill;
    }



    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnGUI()
    //{
    //    if (isSelecting)
    //    {
    //        // Create a rect from both mouse positions
    //        var rect = Utils.GetScreenRect(startMousePos, Input.mousePosition);
    //        Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
    //        Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
    //    }
    //}



    #endregion
}

public static class Utils
{
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}
