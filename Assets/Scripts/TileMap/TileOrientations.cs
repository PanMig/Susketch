using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOrientations : MonoBehaviour
{
    public static TileOrientations Instance;
    public Sprite[] firstFloorOrientations;

    private void Awake()
    {
        Instance = this;
    }
}
