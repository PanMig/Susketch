using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using UnityEngine.UI;
using System;

public class MiniMap : MonoBehaviour
{
    private TileMapView tileMapView;
    [SerializeField] private GameObject prefab;
    private TileMap map;

    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += Init;
        AuthoringTool.onMapSuggestionsReady += SetMiniMap;
    }

    public void Start()
    {
        tileMapView = GetComponent<TileMapView>();
        map = new TileMap();
        map.InitTileMap(tileMapView.gridRect.transform);
    }

    public void Init()
    {
       
    }

    public void CreateMiniMap()
    {
        //map.SetTileMap(AuthoringTool.tileMapMain.GetTileMap());
        //map.RenderTileMap();
    }

    public void SetMiniMap(Tile[,] balancedMap)
    {
        map.SetTileMap(balancedMap, tileMapView.gridRect.transform);
        map.RenderTileMap(tileMapView.gridRect.transform);
    }
}
