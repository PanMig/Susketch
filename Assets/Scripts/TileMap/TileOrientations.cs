using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOrientations : MonoBehaviour
{
    public static TileOrientations Instance;
    public Sprite[] TopOrientations;
    public Sprite[] BottomOrientations;
    public Sprite[] MiddleOrientations;

    private void Awake()
    {
        Instance = this;
    }
}
