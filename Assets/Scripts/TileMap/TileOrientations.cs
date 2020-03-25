using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOrientations : MonoBehaviour
{
    public static TileOrientations Instance;
    public Sprite[] TopOrientations;
    public Sprite[] BottomOrientations;
    public Sprite[] MiddleOrientations;

    public Dictionary<string, char[,]> ruleTiles = new Dictionary<string, char[,]>();


    private void Awake()
    {
        InitDictOfTileRules();
    }

    public void InitDictOfTileRules()
    {
        char[,] leftCornerBottom = new char[,]
        {
                {'A','P','P'},
                {'I','X','P'},
                {'A','I','A'}
        };
        ruleTiles.Add("leftCornerBottom", leftCornerBottom);

        char[,] leftCornerBottomDot = new char[,]
        {
                {'A','P','I'},
                {'I','X','P'},
                {'A','I','A'}
        };
        ruleTiles.Add("leftCornerBottomDot", leftCornerBottomDot);

        char[,] leftCornerTop = new char[,]
        {
                {'A', 'I', 'A'},
                {'I', 'X', 'P'},
                {'A', 'P', 'P'},
        };
        ruleTiles.Add("leftCornerTop", leftCornerTop);

        char[,] leftCornerTopDot = new char[,]
        {
                {'A', 'I', 'A'},
                {'I', 'X', 'P'},
                {'A', 'P', 'I'},
        };
        ruleTiles.Add("leftCornerTopDot", leftCornerTopDot);

        char[,] rightCornerBottom = new char[,]
        {
                {'P', 'P', 'A'},
                {'P', 'X', 'I'},
                {'A', 'I', 'A'},
        };
        ruleTiles.Add("rightCornerBottom", rightCornerBottom);

        char[,] rightCornerBottomDot = new char[,]
        {
                {'I', 'P', 'A'},
                {'P', 'X', 'I'},
                {'A', 'I', 'A'},
        };
        ruleTiles.Add("rightCornerBottomDot", rightCornerBottomDot);

        char[,] rightCornerTop = new char[,]
        {
                {'A', 'I', 'A'},
                {'P', 'X', 'I'},
                {'P', 'P', 'A'},
        };
        ruleTiles.Add("rightCornerTop", rightCornerTop);

        char[,] rightCornerTopDot = new char[,]
        {
                {'A', 'I', 'A'},
                {'P', 'X', 'I'},
                {'I', 'P', 'A'},
        };
        ruleTiles.Add("rightCornerTopDot", rightCornerTopDot);

        char[,] left = new char[,]
        {
                {'A', 'P', 'A'},
                {'I', 'X', 'P'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("left", left);

        char[,] right = new char[,]
        {
                {'A', 'P', 'A'},
                {'P', 'X', 'I'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("right", right);

        char[,] top = new char[,]
        {
                {'A', 'I', 'A'},
                {'P', 'X', 'P'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("top", top);

        char[,] bottom = new char[,]
        {
                {'A', 'P', 'A'},
                {'P', 'X', 'P'},
                {'A', 'I', 'A'},
        };
        ruleTiles.Add("bottom", bottom);

        char[,] rightLeft = new char[,]
        {
                {'A', 'P', 'A'},
                {'I', 'X', 'I'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("rightLeft", rightLeft);

        char[,] topBottom = new char[,]
        {
                {'A', 'I', 'A'},
                {'P', 'X', 'P'},
                {'A', 'I', 'A'},
        };
        ruleTiles.Add("topBottom", topBottom);

        char[,] empty = new char[,]
        {
                {'A', 'P', 'A'},
                {'P', 'X', 'P'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("empty", empty);

        char[,] emptyLeftTopDot = new char[,]
        {
                {'I', 'P', 'A'},
                {'P', 'X', 'P'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("emptyLeftTopDot", emptyLeftTopDot);

        char[,] emptyLeftBottomDot = new char[,]
        {
                {'A', 'P', 'A'},
                {'P', 'X', 'P'},
                {'I', 'P', 'A'},
        };
        ruleTiles.Add("emptyLeftBottomDot", emptyLeftBottomDot);

        char[,] emptyRightTopDot = new char[,]
        {
                {'A', 'P', 'I'},
                {'P', 'X', 'P'},
                {'A', 'P', 'A'},
        };
        ruleTiles.Add("emptyRightTopDot", emptyRightTopDot);

        char[,] emptyRightBottom = new char[,]
        {
                {'A', 'P', 'A'},
                {'P', 'X', 'P'},
                {'A', 'P', 'I'},
        };
        ruleTiles.Add("emptyRightBottom", emptyRightBottom);

        char[,] full = new char[,]
        {
                {'A', 'I', 'A'},
                {'I', 'X', 'I'},
                {'A', 'I', 'A'},
        };
        ruleTiles.Add("full", full);

        char[,] fullDot = new char[,]
        {
                {'I', 'P', 'I'},
                {'P', 'X', 'P'},
                {'I', 'P', 'I'},
        };
        ruleTiles.Add("fullDot", fullDot);
    }
}
