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

    public static Dictionary<char[,], Sprite> ruleTiles = new Dictionary<char[,], Sprite>(new MyEqualityComparer());


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
        char[,] leftCornerBottom = new char[,]
        {
            {'A','P','A'},
            {'I','X','P'},
            {'A','I','A'}
        };
        ruleTiles.Add(leftCornerBottom, mainTiles[2, 0]);

        char[,] leftCornerTop = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(leftCornerTop, mainTiles[0, 0]);

        char[,] rightCornerBottom = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(rightCornerBottom, mainTiles[2, 2]);

        char[,] rightCornerTop = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(rightCornerTop, mainTiles[0, 2]);

        char[,] left = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(left, mainTiles[1, 0]);

        char[,] left_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(left_full, secondartTiles[1, 0]);

        char[,] right = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(right, mainTiles[1, 2]);

        char[,] right_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(right_full, secondartTiles[1, 2]);

        char[,] top = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(top, mainTiles[0, 1]);

        char[,] top_full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(top_full, secondartTiles[0, 1]);

        char[,] bottom = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(bottom, mainTiles[2, 1]);

        char[,] bottom_full = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(bottom_full, secondartTiles[2, 1]);

        char[,] rightLeft = new char[,]
        {
            {'A', 'P', 'A'},
            {'I', 'X', 'I'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(rightLeft, secondartTiles[2, 2]);

        char[,] topBottom = new char[,]
        {
            {'A', 'I', 'A'},
            {'P', 'X', 'P'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(topBottom, secondartTiles[0, 0]);

        char[,] empty = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'A'},
        };
        ruleTiles.Add(empty, mainTiles[1, 1]);

        char[,] emptyBottomRight = new char[,]
        {
            {'A', 'P', 'A'},
            {'P', 'X', 'P'},
            {'A', 'P', 'I'},
        };
        ruleTiles.Add(emptyBottomRight, secondartTiles[0, 2]);

        char[,] full = new char[,]
        {
            {'A', 'I', 'A'},
            {'I', 'X', 'I'},
            {'A', 'I', 'A'},
        };
        ruleTiles.Add(full, secondartTiles[2, 0]);
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

public class MyEqualityComparer : IEqualityComparer<char[,]>
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

    //public int GetHashCode(char[,] obj)
    //{
    //    int hash = 17;
    //    string hashS = "";
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            if (obj[i, j] != 'A')
    //            {
    //                hash = 31 * obj[i, j];
    //            }
    //        }
    //    }
    //    Debug.Log("Hash code in dict:" + hash);
    //    return hash;
    //}

    public int GetHashCode(char[,] obj)
    {
        int hash = 17;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (obj[i, j] != 'A')
                {
                    hash = hash * 31 + obj[i, j];
                }
            }
        }
        Debug.Log("Hash code in dict:" + hash);
        return hash;
    }
}
