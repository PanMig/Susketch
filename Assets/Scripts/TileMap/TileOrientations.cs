using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileOrientations : SerializedMonoBehaviour
{
    public Sprite[] TopOrientations;

    [TableMatrix(HorizontalTitle = "Main tiles matrix", SquareCells = false, Transpose = true)]
    public Sprite[,] mainTiles = new Sprite[3, 3];

    [TableMatrix(HorizontalTitle = "Main tiles matrix", SquareCells = false, Transpose = true)]
    public Sprite[,] secondartTiles = new Sprite[3, 3];

    [TableMatrix(HorizontalTitle = "Main tiles matrix", SquareCells = false, Transpose = true)]
    public Sprite[,] diagonal = new Sprite[4, 4];

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
            {'A','P','A'},
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
        dict_LB.Add(LCB_R1, diagonal[0,0]);

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
            {'A', 'P', 'A'},
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
        //dict_RB.Add(RCB_L1, diagonal[0,1]);

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
            {'A', 'P', 'A'},
        };

        char[,] LCT_2 = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'I'},
        };

        var dict_LCT = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_LCT.Add(LCT_1, mainTiles[0, 0]);

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
            {'A', 'P', 'A'},
        };
        var dict_RCT = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dict_RCT.Add(RCT_1, mainTiles[0,2]);


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
            {'A', 'P', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        var dictLeft = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictLeft.Add(L_1, mainTiles[1,0]);

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
        dictLeftFull.Add(LF_1, secondartTiles[1,0]);

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
            {'A', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        var dictRight = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictRight.Add(R_1, mainTiles[1,2]);

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
        dict_RF.Add(RF_1, secondartTiles[1,2]);

        char[,] right_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(right_full, dict_RF);

        #endregion

        #region Top

        char[,] Top_1 = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };

        var dictTop = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictTop.Add(Top_1, mainTiles[0,1]);

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
        dictTopFull.Add(TF_1, secondartTiles[0,1]);

        char[,] top_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(top_full, dictTopFull);

        #endregion

        #region Bottom

        char[,] Bottom_1 = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };

        var dictBottom = new Dictionary<char[,], Sprite>((new SpriteDictEC()));
        dictBottom.Add(Bottom_1, mainTiles[2,1]);

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
        dictBF.Add(BF_1, secondartTiles[2,1]);

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
        dictRL.Add(RL, secondartTiles[2, 2]);

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
        dictTopBottom.Add(TP_1, secondartTiles[0,0]);

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
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        var dictEmpty = new Dictionary<char[,], Sprite>(new SpriteDictEC());
        dictEmpty.Add(Empty_1, mainTiles[1,1]);

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
        dictFull.Add(Full_1, secondartTiles[2,0]);

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
                if (obj[i, j] == 'A')
                {
                    continue;
                }
                hash = hash * 31 + obj[i, j];
            }
        }
        return hash;
    }
}
