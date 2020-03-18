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
    private TileMapView _tileMapView;
    private TileMap _map;
    private MiniMapView _mapView;
    [SerializeField] private int _index;
    private AuthoringTool authTool;
    private float _newBlueAmount = 0;
    private float _newRedAmount= 0;


    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += Init;
        AuthoringTool.onMapSuggestionsReady += SetMiniMap;
    }

    public void Init()
    {
        authTool = GameObject.FindGameObjectWithTag("authoring tool").GetComponent<AuthoringTool>();
        _tileMapView = GetComponent<TileMapView>();
        _map = new TileMap();
        _map.Init();
        _map.PaintTiles(_tileMapView.gridRect.transform,0.1f);
        _mapView = GetComponent<MiniMapView>();
    }

    public void ApplyToTileMapMain()
    {
        if (_map.GetTileMap() != null)
        {
            tileMapMain.SetTileMap(_map.GetTileMap());
            authTool.InvokeMetrics();
        }
    }

    public void SetMiniMap(List<KeyValuePair<TileMap, float>> balancedMaps)
    {
        _map.SetTileMap(balancedMaps[_index].Key.GetTileMap());
        _map.Render();
        var percent = balancedMaps[_index].Value;
        Destroy(MapSuggestionMng.tempView);
        SetMiniMapView(percent);
    }

    public void SetMiniMapView(float percent)
    {
        _newBlueAmount = (1 - percent) * 100;
        _newRedAmount = percent * 100;
        var curBlueAmount = (1 - currKillRatio) * 100;
        var curRedAmount = currKillRatio * 100;

        var blueSign = (curBlueAmount - _newBlueAmount) < 0 ? "-" : "+";
        var redSign = (curRedAmount - _newRedAmount) < 0 ? "-" : "+";

        _mapView.SetKillRatioBar(_newBlueAmount/100.0f, _newRedAmount / 100.0f);
        //use three whitespaces for better alignment.
        _mapView.BluePercentText.text = $"{_newBlueAmount.ToString($"F0")} %   ({blueSign}{(curBlueAmount - _newBlueAmount).ToString("F0")}%)";
        _mapView.RedPercentText.text = $"{_newRedAmount.ToString($"F0")} %   ({redSign}{(curRedAmount - _newRedAmount).ToString("F0")}%)";

        var result = CalculateRatioDifference(percent, currKillRatio);
        if (result < 0)
        {
            _mapView.ResultsText.text = $"+{Mathf.Abs(result).ToString($"F1")} % improvement";
            _mapView.ResultsText.color = Color.green;
        }
        else
        {
            _mapView.ResultsText.text = $"-{result.ToString($"F1")} % loss";
            _mapView.ResultsText.color = Color.red;
        }
    }

    private float CalculateRatioDifference(float newPercent, float curPercent)
    {
        var suggestedKR = Mathf.Abs(0.5f - newPercent);
        var currentKR = Mathf.Abs(0.5f - curPercent);
        if (currentKR == 0.0f) return 0.0f;
        // we divide by the desired number(0.5) so we get the percentage error (mathematical measurement).
        var result = (suggestedKR - currentKR) / 0.5f;
        return result * 100;
    }
}
