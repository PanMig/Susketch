using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using UnityEngine.UI;
using System;
using static AuthoringTool;
using Michsky.UI.ModernUIPack;
using UnityEngine.EventSystems;
using TMPro;

public class MiniMap : MonoBehaviour
{
    private TileMapView tileMapView;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private GameObject prefab; 
    [SerializeField] private ModalWindowManager modalWindow;
    [SerializeField] private int index;
    private TileMap map;

    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += Init;
        AuthoringTool.onMapSuggestionsReady += SetMiniMap;
    }

    public void Init()
    {
        tileMapView = GetComponent<TileMapView>();
        map = new TileMap();
        map.Init();
        map.PaintTiles(tileMapView.gridRect.transform,0.1f);
    }

    public void SetMiniMap(List<KeyValuePair<TileMap, float>> balancedMaps)
    {
        map.SetTileMap(balancedMaps[index].Key.GetTileMap());
        float percent = balancedMaps[index].Value;
        map.Render();
        float blueAmount = (1 - percent)*100;
        float redAmount = percent * 100;
        resultsText.text = "Blue: " + blueAmount.ToString("F1") + "%  " + "Red: " + redAmount.ToString("F1") + "%";
        Destroy(MapSuggestionMng.tempView);
    }
}
