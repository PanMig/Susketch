using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileOrientations : SerializedMonoBehaviour
{
    public Sprite[] diagonalTop = new Sprite[4];
    public Sprite[] diagonalBottom = new Sprite[4];
    public Sprite[] diagonalLeft = new Sprite[3];
    public Sprite[] diagonalRight = new Sprite[3];
    public Sprite[] Bidirectional = new Sprite[2];

    [TableMatrix(HorizontalTitle = "Main Quad tiles", SquareCells = false, Transpose = true)]
    public Sprite[,] mainTiles = new Sprite[3, 3];

    [TableMatrix(HorizontalTitle = "Full borders tiles", SquareCells = false, Transpose = true)]
    public Sprite[,] fullBorderTiles = new Sprite[3, 3];

    [TableMatrix(HorizontalTitle = "Empty With Border One", SquareCells = true, Transpose = true)]
    public Sprite[,] empty_1 = new Sprite[1, 4];

    [TableMatrix(HorizontalTitle = "Empty With Border Two", SquareCells = false, Transpose = true)]
    public Sprite[,] empty_2b = new Sprite[2, 4];

    [TableMatrix(HorizontalTitle = "Empty With Border Three", SquareCells = true, Transpose = true)]
    public Sprite[,] empty_3 = new Sprite[1, 4];

    public static Dictionary<char[,], Dictionary<char[,], Sprite>> ruleTiles = new Dictionary<char[,], Dictionary<char[,], Sprite>>(new TemplateDictEC());

    public static Dictionary<char[,], Sprite> sections = new Dictionary<char[,], Sprite>(new SpriteDictEC());


    private void Awake()
    {
        InitDictOfTileRules();

        //add extra rules procedurally.
        //foreach (var key in ruleTiles.Keys)
        //{
        //    var copyP = (char[,])key.Clone();
        //    var copyI = (char[,])key.Clone();

        //    for (int i = 0; i < 3; i++)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            if (key[i, j] == 'A')
        //            {
        //                copyP[i, j] = 'P';
        //            }
        //            ruleTiles.Add(copyP, ruleTiles[key]);
        //        }
        //    }
        //}
    }

    public void InitDictOfTileRules()
    {
        #region LeftCornerBottom

        char[,] LB = new char[,]
        {
            {'A','P','P'},
            {'I','X','P'},
            {'A','I','A'}
        };

        char[,] LCB_R1= new char[,]
        {
            {'A','P','I'},
            {'I','X','P'},
            {'A','I','A'}
        };

        var dict_LB = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_LB.Add(LB, mainTiles[2,0]);
        dict_LB.Add(LCB_R1, fullBorderTiles[2,0]);

        //main dict template
        char[,] leftCornerBottom = new char[,]
        {
            {'A','P','A'},
            {'I','X','P'},
            {'A','I','A'}
        };
        ruleTiles.Add(leftCornerBottom, dict_LB);

        #endregion

        #region RightCornerBottom

        char[,] RCB = new char[,]
        {
            {'P', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };

        char[,] RCB_L1 = new char[,]
        {
            {'I', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };

        var dict_RB = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_RB.Add(RCB, mainTiles[2,2]);
        dict_RB.Add(RCB_L1, fullBorderTiles[2,2]);

        char[,] rightCornerBottom = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(rightCornerBottom, dict_RB);

        #endregion

        #region LeftCornerTop

        char[,] LCT_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'P'},
        };

        char[,] LCT_2 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'I'},
        };

        var dict_LCT = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_LCT.Add(LCT_1, mainTiles[0, 0]);
        dict_LCT.Add(LCT_2, fullBorderTiles[0, 0]);

        char[,] leftCornerTop = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(leftCornerTop, dict_LCT);

        #endregion

        #region RightCornerTop

        char[,] RCT_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'P', 'P', 'A'},
        };

        char[,] RCT_2 = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'I', 'P', 'A'},
        };
        var dict_RCT = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_RCT.Add(RCT_1, mainTiles[0,2]);
        dict_RCT.Add(RCT_2, fullBorderTiles[0,2]);


        char[,] rightCornerTop = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(rightCornerTop, dict_RCT);

        #endregion

        #region Left

        char[,] L_1 = new char[,]
        {
            {'A', 'P', 'P'},
            {'I', 'X', 'P'},
            {'A', 'P', 'P'},
        };
        char[,] L_T = new char[,]
        {
            {'A', 'P', 'I'},
            {'I', 'X', 'P'},
            {'A', 'P', 'P'},
        };
        char[,] L_B = new char[,]
        {
            {'A', 'P', 'P'},
            {'I', 'X', 'P'},
            {'A', 'P', 'I'},
        };
        char[,] L_TB = new char[,]
        {
            {'A', 'P', 'I'},
            {'I', 'X', 'P'},
            {'A', 'P', 'I'},
        };
        var dictLeft = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictLeft.Add(L_1, mainTiles[1,0]);
        dictLeft.Add(L_T, diagonalLeft[0]);
        dictLeft.Add(L_B, diagonalLeft[1]);
        dictLeft.Add(L_TB, diagonalLeft[2]);

        char[,] left = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(left, dictLeft);

        #endregion

        #region LeftFull

        char[,] LF_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        var dictLeftFull = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictLeftFull.Add(LF_1, fullBorderTiles[1,0]);

        char[,] left_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(left_full, dictLeftFull);

        #endregion

        #region Right

        char[,] R_1 = new char[,]
        {
            {'P', 'P', 'A'},
            {'P', 'X', 'I'},
            {'P', 'P', 'A'},
        };
        char[,] R_T = new char[,]
        {
            {'I', 'P', 'A'},
            {'P', 'X', 'I'},
            {'P', 'P', 'A'},
        };
        char[,] R_B = new char[,]
        {
            {'P', 'P', 'A'},
            {'P', 'X', 'I'},
            {'I', 'P', 'A'},
        };
        char[,] R_TB = new char[,]
        {
            {'I', 'P', 'A'},
            {'P', 'X', 'I'},
            {'I', 'P', 'A'},
        };
        var dictRight = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictRight.Add(R_1, mainTiles[1,2]);
        dictRight.Add(R_T, diagonalRight[0]);
        dictRight.Add(R_B, diagonalRight[1]);
        dictRight.Add(R_TB, diagonalRight[2]);

        char[,] right = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(right, dictRight);

        #endregion

        #region RightFull

        char[,] RF_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        var dict_RF = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_RF.Add(RF_1, fullBorderTiles[1,2]);

        char[,] right_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(right_full, dict_RF);

        #endregion

        #region Top

        char[,] Top = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'P', 'P', 'P'},
        };

        char[,] Top_R = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'I', 'P', 'P'},
        };

        char[,] Top_L = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'P', 'P', 'I'},
        };

        char[,] Top_RL = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'I', 'P', 'I'},
        };

        var dictTop = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictTop.Add(Top, mainTiles[0,1]);
        dictTop.Add(Top_R, diagonalTop[0]);
        dictTop.Add(Top_L, diagonalTop[1]);
        dictTop.Add(Top_RL, diagonalTop[2]);

        char[,] top = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(top, dictTop);

        #endregion

        #region TopFull

        char[,] TF_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        Dictionary<char[,], Sprite> dictTopFull = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictTopFull.Add(TF_1, fullBorderTiles[0,1]);

        char[,] top_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(top_full, dictTopFull);

        #endregion

        #region Bottom

        char[,] Bottom = new char[,]
        {
            {'P', 'P', 'P'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        char[,] Bottom_R = new char[,]
        {
            {'P', 'P', 'I'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        char[,] Bottom_L = new char[,]
        {
            {'I', 'P', 'P'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        char[,] Bottom_RL = new char[,]
        {
            {'I', 'P', 'I'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        var dictBottom = new Dictionary<char[,], Sprite>((new SpriteDictEC()));
        dictBottom.Add(Bottom, mainTiles[2,1]);
        dictBottom.Add(Bottom_R, diagonalBottom[0]);
        dictBottom.Add(Bottom_L, diagonalBottom[1]);
        dictBottom.Add(Bottom_RL, diagonalBottom[2]);

        char[,] bottom = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(bottom, dictBottom);

        #endregion

        #region BottomFull

        char[,] BF_1 = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        var dictBF = new Dictionary<char[,], Sprite>((new SpriteDictEC()));
        dictBF.Add(BF_1, fullBorderTiles[2,1]);

        char[,] bottom_full = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(bottom_full, dictBF);

        #endregion

        #region RightLeft

        char[,] RL = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        var dictRL = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictRL.Add(RL, Bidirectional[1]);

        char[,] rightLeft = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(rightLeft, dictRL);

        #endregion

        #region TopBottom

        char[,] TP_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        var dictTopBottom = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictTopBottom.Add(TP_1, Bidirectional[0]);

        char[,] topBottom = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(topBottom, dictTopBottom);

        #endregion

        #region Empty

        char[,] Empty_1 = new char[,]
        {
            {'P', 'P', 'P'},
            {'P', 'X', 'P'},
            {'P', 'P', 'P'},
        };
        char[,] Empty_2 = new char[,]
        {
            {'I', 'P', 'P'},
            {'P', 'X', 'P'},
            {'P', 'P', 'P'},
        };
        char[,] Empty_3 = new char[,]
        {
            {'P', 'P', 'I'},
            {'P', 'X', 'P'},
            {'P', 'P', 'P'},
        };
        char[,] Empty_4 = new char[,]
        {
            {'P', 'P', 'P'},
            {'P', 'X', 'P'},
            {'I', 'P', 'P'},
        };
        char[,] Empty_5 = new char[,]
        {
            {'P', 'P', 'P'},
            {'P', 'X', 'P'},
            {'P', 'P', 'I'},
        };
        char[,] Empty_6 = new char[,]
        {
            {'I', 'P', 'I'},
            {'P', 'X', 'P'},
            {'P', 'P', 'P'},
        };
        char[,] Empty_7 = new char[,]
        {
            {'I', 'P', 'P'},
            {'P', 'X', 'P'},
            {'P', 'P', 'I'},
        };
        char[,] Empty_8 = new char[,]
        {
            {'I', 'P', 'P'},
            {'P', 'X', 'P'},
            {'I', 'P', 'P'},
        };
        char[,] Empty_9 = new char[,]
        {
            {'P', 'P', 'I'},
            {'P', 'X', 'P'},
            {'P', 'P', 'I'},
        };
        char[,] Empty_10 = new char[,]
        {
            {'P', 'P', 'I'},
            {'P', 'X', 'P'},
            {'I', 'P', 'P'},
        };
        char[,] Empty_11 = new char[,]
        {
            {'I', 'P', 'I'},
            {'P', 'X', 'P'},
            {'I', 'P', 'I'},
        };
        char[,] Empty_12 = new char[,]
        {
            {'P', 'P', 'P'},
            {'P', 'X', 'P'},
            {'I', 'P', 'I'},
        };
        char[,] triplex_L = new char[,]
        {
            {'I', 'P', 'P'},
            {'P', 'X', 'P'},
            {'I', 'P', 'I'},
        };
        char[,] triplex_LReversed = new char[,]
        {
            {'P', 'P', 'I'},
            {'P', 'X', 'P'},
            {'I', 'P', 'I'},
        };
        char[,] triplex_G = new char[,]
        {
            {'I', 'P', 'I'},
            {'P', 'X', 'P'},
            {'I', 'P', 'P'},
        };
        char[,] triplex_GReversed = new char[,]
        {
            {'I', 'P', 'I'},
            {'P', 'X', 'P'},
            {'P', 'P', 'I'},
        };
        var dictEmpty = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictEmpty.Add(Empty_1, mainTiles[1,1]);
        dictEmpty.Add(Empty_2, empty_1[0,0]);
        dictEmpty.Add(Empty_3, empty_1[0,1]);
        dictEmpty.Add(Empty_4, empty_1[0,2]);
        dictEmpty.Add(Empty_5, empty_1[0,3]);
        dictEmpty.Add(Empty_6, empty_2b[0,0]);
        dictEmpty.Add(Empty_7, empty_2b[0,1]);
        dictEmpty.Add(Empty_8, empty_2b[0,2]);
        dictEmpty.Add(Empty_9, empty_2b[0,3]);
        dictEmpty.Add(Empty_10, empty_2b[1,0]);
        dictEmpty.Add(Empty_11, empty_2b[1,1]);
        dictEmpty.Add(Empty_12, empty_2b[1,2]);
        dictEmpty.Add(triplex_L, empty_3[0,0]);
        dictEmpty.Add(triplex_LReversed, empty_3[0,1]);
        dictEmpty.Add(triplex_G, empty_3[0,2]);
        dictEmpty.Add(triplex_GReversed, empty_3[0,3]);

        char[,] empty = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        
        ruleTiles.Add(empty, dictEmpty);

        #endregion

        #region Full

        char[,] Full_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };

        var dictFull = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictFull.Add(Full_1, fullBorderTiles[1,1]);

        char[,] full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(full, dictFull);

        #endregion
    }


    //public void InitDictOfTileRules()
    //{
    //    char[,] leftCornerBottom = new char[,]
    //    {
    //            {'A','P','P'},
    //            {'I','X','P'},
    //            {'A','I','A'}
    //    };
    //    ruleTiles.Add("leftCornerBottom", leftCornerBottom);

    //    char[,] leftCornerBottomDot = new char[,]
    //    {
    //            {'A','P','I'},
    //            {'I','X','P'},
    //            {'A','I','A'}
    //    };
    //    ruleTiles.Add("leftCornerBottomDot", leftCornerBottomDot);

    //    char[,] leftCornerTop = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'I', 'X', 'P'},
    //            {'A', 'P', 'P'},
    //    };
    //    ruleTiles.Add("leftCornerTop", leftCornerTop);

    //    char[,] leftCornerTopDot = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'I', 'X', 'P'},
    //            {'A', 'P', 'I'},
    //    };
    //    ruleTiles.Add("leftCornerTopDot", leftCornerTopDot);

    //    char[,] rightCornerBottom = new char[,]
    //    {
    //            {'P', 'P', 'A'},
    //            {'P', 'X', 'I'},
    //            {'A', 'I', 'A'},
    //    };
    //    ruleTiles.Add("rightCornerBottom", rightCornerBottom);

    //    char[,] rightCornerBottomDot = new char[,]
    //    {
    //            {'I', 'P', 'A'},
    //            {'P', 'X', 'I'},
    //            {'A', 'I', 'A'},
    //    };
    //    ruleTiles.Add("rightCornerBottomDot", rightCornerBottomDot);

    //    char[,] rightCornerTop = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'P', 'X', 'I'},
    //            {'P', 'P', 'A'},
    //    };
    //    ruleTiles.Add("rightCornerTop", rightCornerTop);

    //    char[,] rightCornerTopDot = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'P', 'X', 'I'},
    //            {'I', 'P', 'A'},
    //    };
    //    ruleTiles.Add("rightCornerTopDot", rightCornerTopDot);

    //    char[,] left = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'I', 'X', 'P'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("left", left);

    //    char[,] right = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'P', 'X', 'I'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("right", right);

    //    char[,] top = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("top", top);

    //    char[,] bottom = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'I', 'A'},
    //    };
    //    ruleTiles.Add("bottom", bottom);

    //    char[,] rightLeft = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'I', 'X', 'I'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("rightLeft", rightLeft);

    //    char[,] topBottom = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'I', 'A'},
    //    };
    //    ruleTiles.Add("topBottom", topBottom);

    //    char[,] empty = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("empty", empty);

    //    char[,] emptyLeftTopDot = new char[,]
    //    {
    //            {'I', 'P', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("emptyLeftTopDot", emptyLeftTopDot);

    //    char[,] emptyLeftBottomDot = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'P', 'X', 'P'},
    //            {'I', 'P', 'A'},
    //    };
    //    ruleTiles.Add("emptyLeftBottomDot", emptyLeftBottomDot);

    //    char[,] emptyRightTopDot = new char[,]
    //    {
    //            {'A', 'P', 'I'},
    //            {'P', 'X', 'P'},
    //            {'A', 'P', 'A'},
    //    };
    //    ruleTiles.Add("emptyRightTopDot", emptyRightTopDot);

    //    char[,] emptyRightBottom = new char[,]
    //    {
    //            {'A', 'P', 'A'},
    //            {'P', 'X', 'P'},
    //            {'A', 'P', 'I'},
    //    };
    //    ruleTiles.Add("emptyRightBottom", emptyRightBottom);

    //    char[,] full = new char[,]
    //    {
    //            {'A', 'I', 'A'},
    //            {'I', 'X', 'I'},
    //            {'A', 'I', 'A'},
    //    };
    //    ruleTiles.Add("full", full);

    //    char[,] fullDot = new char[,]
    //    {
    //            {'I', 'P', 'I'},
    //            {'P', 'X', 'P'},
    //            {'I', 'P', 'I'},
    //    };
    //    ruleTiles.Add("fullDot", fullDot);
    //}

    //foreach (var key in ruleTiles.Keys.ToList())
    //{
    //    var shallowCopyP = (char[,])key.Clone();
    //    var shallowCopyI = (char[,])key.Clone();

    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            if (key[i, j] == 'A')
    //            {
    //                shallowCopyP[i, j] = 'P';
    //            }
    //        }
    //    }

    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            if (key[i, j] == 'A')
    //            {
    //                shallowCopyI[i, j] = 'I';
    //            }
    //        }
    //    }
    //    ruleTiles.Add(shallowCopyP, ruleTiles[key]);
    //    ruleTiles.Add(shallowCopyI, ruleTiles[key]);
    //    ruleTiles.Remove(key);
    //}
}


public class TemplateDictEC : IEqualityComparer<char[,]>
{
    public bool Equals(char[,] x, char[,] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (x[i, j] == 'A' || y[i, j] == 'A')
                {
                    continue;
                }
                if (x[i, j] != y[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetHashCode(char[,] obj)
    {
        int hash = 17;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 0 & j == 0 || i == 0 && j == 2 ||
                    i == 2 & j == 0 || i == 2 && j == 2)
                {
                    continue;
                }
                hash = hash * 31 + obj[i, j];
            }
        }
        return hash;
    }
}

public class SpriteDictEC : IEqualityComparer<char[,]>
{
    public bool Equals(char[,] x, char[,] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (x[i, j] == 'A' || y[i, j] == 'A')
                {
                    continue;
                }
                if (x[i, j] != y[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetHashCode(char[,] obj)
    {
        int hash = 17;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 0 & j == 0 || i == 0 && j == 2 ||
                    i == 2 & j == 0 || i == 2 && j == 2)
                {
                    continue;
                }
                hash = hash * 31 + obj[i, j];
            }
        }
        return hash;
    }
}
