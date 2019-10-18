using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private TileMap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponentInChildren<TileMap>();
        tileMap.InitTileMap();
        tileMap.InitRegions();
        tileMap.PaintRegion(3, 0, 1);
        tileMap.PaintRegion(0, 3, 1);
    }
}
