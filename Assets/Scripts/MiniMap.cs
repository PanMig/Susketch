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

    }

    public void Init()
    {
        tileMapView = GetComponent<TileMapView>();
        map = new TileMap();
        map.Init();
        map.PaintTiles(tileMapView.gridRect.transform,0.1f);
    }

    public void CreateMiniMap()
    {
        //map.SetTileMap(AuthoringTool.tileMapMain.GetTileMap());
        //map.RenderTileMap();
    }

    public void SetMiniMap(Tile[,] balancedMap)
    {
        
        map.SetTileMap(balancedMap);
        map.Render();
        balancedMap = null;
        DestroyImmediate(MapSuggestionMng.tempView);
    }
}
