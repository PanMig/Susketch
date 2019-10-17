using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthoringTool : MonoBehaviour
{
    private TileMap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponentInChildren<TileMap>();
        tileMap.InitTileMap();   
    }
}
