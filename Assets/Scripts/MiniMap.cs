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
    [SerializeField] private Button applyBtn;
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
        applyBtn = GetComponentInChildren<Button>();
        applyBtn.interactable = false;
    }

    public void ApplyToTileMapMain()
    {
        if (map.GetTileMap() != null)
        {
            tileMapMain.SetTileMap(map.GetTileMap());
        }
    }

    public void SetMiniMap(List<KeyValuePair<TileMap, float>> balancedMaps)
    {
        map.SetTileMap(balancedMaps[index].Key.GetTileMap());
        float percent = balancedMaps[index].Value;
        map.Render();
        float newBlueAmount = (1 - percent) * 100;
        float newRedAmount = percent * 100;
        float curBlueAmount = (1 - currKillRatio) * 100;
        float curRedAmount =  currKillRatio * 100;
        char blueSign, redSign;

        if ((curBlueAmount - newBlueAmount) < 0)
        {
            blueSign = '-'; 
        }
        else
        {
            blueSign = '+';
        }
        if ((curRedAmount - newRedAmount) < 0)
        {
            redSign = '-';
        }
        else
        {
            redSign = '+';
        }
        resultsText.text = $"Blue: {newBlueAmount.ToString("F0")} % ({blueSign}{(curBlueAmount - newBlueAmount).ToString("F0")}%) \n" +
                           $"Red: {newRedAmount.ToString("F0")} % ({redSign}{(curRedAmount - newRedAmount).ToString("F0")}%)";
        applyBtn.interactable = true;
        Destroy(MapSuggestionMng.tempView);
    }
}
