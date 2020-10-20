using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TileMapLogic;
using static AuthoringTool;

public class TileCursor : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private Color startcolor;
    private Tile selectedTile;
    private bool dragging = false;
    private bool bucketPainting = false;
    
    public enum CursorType
    {
        tile,
        decoration,
        fill,
    }

    public static CursorType currentCursorType;

    #region interface implementations

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
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
        // fire predictions event.
        if(dragging == false) { EventManagerUI.onMapReadyForPrediction?.Invoke(); }
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
        //HighlightHoveredTile(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //UnHighlightHoveredTile();
    }

    private void UnHighlightHoveredTile()
    {
        selectedTile.SetColor(startcolor);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        EventManagerUI.onMapReadyForPrediction?.Invoke();
    }

    #endregion

    private void FillBoundedArea(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null && !bucketPainting)
        {
            bucketPainting = true;
            int index_row, index_col;
            GetIndexFromCoordinates(eventData, out index_row, out index_col);
            var oldColor = (int) tileMapMain.GetTileWithIndex(index_row, index_col).envTileID;
            Brush.Instance.FillRegion(index_row, index_col, Brush.Instance.brushThemes[Brush.Instance.currTileBrush], Brush.Instance.brushThemes[oldColor]);
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    tileMapMain.FormatTileOrientation(i, j, TileEnums.EnviromentTiles.ground);
                }
            }
            EventManagerUI.onSingleClickEdit?.Invoke();
            bucketPainting = false;
        }

    }

    private static void GetIndexFromCoordinates(PointerEventData eventData, out int index_row, out int index_col)
    {
        // x coordinate corresponds to columns(horizontal) and y corresponds to rows(vertical).
        float x = eventData.pointerCurrentRaycast.gameObject.transform.localPosition.x;
        float y = Mathf.Abs(eventData.pointerCurrentRaycast.gameObject.transform.localPosition.y);

        // Add some extra padding for accurate results.
        index_row = (int)Mathf.Floor(y / 50);
        index_col = (int)Mathf.Floor(x / 52);
    }

    private void HighlightHoveredTile(PointerEventData eventData)
    {
            int index_row, index_col;
            GetIndexFromCoordinates(eventData, out index_row, out index_col);
            selectedTile = tileMapMain.GetTileWithIndex(index_row, index_col);
            startcolor = selectedTile.image.color;
            selectedTile.SetColor(Color.red);
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
            tile.SetTheme(Brush.Instance.brushThemes[index]);
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    tileMapMain.FormatTileOrientation(i,j, TileEnums.EnviromentTiles.ground);
                }
            }
            EventManagerUI.onSingleClickEdit?.Invoke();
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
            tile.SetDecoration(Brush.Instance.decorations[index]);
            if (tile.decID == TileEnums.Decorations.stairs)
            {
                
                tileMapMain.SetStairsOrientationTile(tile);
            }
            else if (tile.decID == TileEnums.Decorations.empty)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        tileMapMain.FormatTileOrientation(i, j, TileEnums.EnviromentTiles.ground);
                    }
                }
            }
            EventManagerUI.onSingleClickEdit?.Invoke();
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

//    private const TileEnums.EnviromentTiles firstFloor = TileEnums.EnviromentTiles.level_1;
//    public enum orientation { standalone, top, bottom, middle };

//    public void FormatTileOrientation(int x, int y, HashSet<Tile> orientedTiles, orientation Orientation)
//    {

//        Tile tile = GetTileWithIndex(x, y);
//        if (tile.envTileID != firstFloor || orientedTiles.Contains(tile))
//        {
//            return;
//        }

//        // 4 Neighbours in oder D , U, R, L.
//        var neighbours = PathUtils.GetNeighboursCross(tile, this);

//        //Standalone Tile : all neighbours not first floor.
//        if (neighbours[0].envTileID != firstFloor && neighbours[1].envTileID != firstFloor &&
//            neighbours[2].envTileID != firstFloor && neighbours[3].envTileID != firstFloor)
//        {
//            // Set Tile Standalone
//            tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[3]);
//        }
//        //Top tile : No first floor on top.
//        else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID != firstFloor)
//        {
//            // Set Tile top
//            tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
//            orientedTiles.Add(tile);
//            FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.top);
//        }
//        //Bottom tile : No first floor below.
//        else if (neighbours[1].envTileID == firstFloor && neighbours[0].envTileID != firstFloor)
//        {
//            // Set Tile top
//            tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
//            orientedTiles.Add(tile);
//            FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.bottom);
//        }
//        //Middle tile
//        else if (neighbours[0].envTileID == firstFloor && neighbours[1].envTileID == firstFloor)
//        {
//            // Set Tile middle.
//            tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
//            orientedTiles.Add(tile);
//            //FormatTileOrientation(tile.X, tile.Y - 1, orientedTiles, orientation.middle);
//            //FormatTileOrientation(tile.X, tile.Y + 1, orientedTiles, orientation.middle);
//            //FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, orientation.middle);
//            //FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, orientation.middle);
//        }
//        //Right
//        if (neighbours[3].envTileID == firstFloor && neighbours[2].envTileID != firstFloor)
//        {
//            orientedTiles.Add(tile);
//            var orientation = GetLeftTileOrientation(tile);
//            //top right tile
//            if (orientation == orientation.top)
//            {
//                tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[2]);
//                var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
//                leftTile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
//                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
//                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
//            }
//            else if (orientation == orientation.bottom)
//            {
//                tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[2]);
//                var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
//                leftTile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
//                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
//                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
//            }
//            else if (orientation == orientation.middle)
//            {
//                tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[2]);
//                var leftTile = GetTileWithIndex(tile.X, tile.Y - 1);
//                leftTile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
//                FormatTileOrientation(tile.X + 1, tile.Y, orientedTiles, 0);
//                FormatTileOrientation(tile.X - 1, tile.Y, orientedTiles, 0);
//            }
//        }
//        //left
//        //else if (neighbours[2].envTileID == firstFloor && neighbours[3].envTileID != firstFloor)
//        //{
//        //    orientedTiles.Add(tile);
//        //    var orientation = GetRightTileOrientation(tile);
//        //    //top right tile
//        //    if (orientation == orientation.top)
//        //    {
//        //        tile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[3]);
//        //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
//        //        rightTile.FormatTileSprite(this, TileOrientations.Instance.TopOrientations[0]);
//        //    }
//        //    else if (orientation == orientation.bottom)
//        //    {
//        //        tile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[3]);
//        //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
//        //        rightTile.FormatTileSprite(this, TileOrientations.Instance.BottomOrientations[0]);
//        //    }
//        //    else if (orientation == orientation.middle)
//        //    {
//        //        tile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[3]);
//        //        var rightTile = GetTileWithIndex(tile.X, tile.Y + 1);
//        //        rightTile.FormatTileSprite(this, TileOrientations.Instance.MiddleOrientations[0]);
//        //    }
//        //}
//    }

//    private orientation GetLeftTileOrientation(Tile tile)
//    {
//        var left = PathUtils.GetNeighbours(tile, this)[3];
//        var neighbours = PathUtils.GetNeighbours(left, this);
//        if (neighbours[1].envTileID != firstFloor)
//        {
//            return orientation.top;
//        }
//        if (neighbours[0].envTileID != firstFloor)
//        {
//            return orientation.bottom;
//        }
//        return orientation.middle;
//    }

//    private orientation GetRightTileOrientation(Tile tile)
//    {
//        var right = PathUtils.GetNeighbours(tile, this)[2];
//        var neighbours = PathUtils.GetNeighbours(right, this);
//        if (neighbours[1].envTileID != firstFloor)
//        {
//            return orientation.top;
//        }
//        if (neighbours[0].envTileID != firstFloor)
//        {
//            return orientation.bottom;
//        }
//        return orientation.middle;
//    }
//}

