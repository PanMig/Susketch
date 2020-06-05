using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using static AuthoringTool;

public class MiniMap : MonoBehaviour
{
    private TileMapView _tileMapView;
    private MiniTileMap _map;
    private MiniMapView _mapView;
    [SerializeField] private int _index;
    [SerializeField] private Enums.PowerUpPlacement type;
    private float _newBlueAmount = 0;
    private float _newRedAmount= 0;

    public delegate void OnMiniMapApply();
    public static event OnMiniMapApply onMiniMapApply;

    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += Init;
        switch (type)
        {
            case Enums.PowerUpPlacement.random:
                AuthoringTool.onMapMutationRandom += SetMiniMap;
                break;
            case Enums.PowerUpPlacement.regionShift:
                AuthoringTool.onMapMutationRegionShift += SetMiniMap;
                break;
            case Enums.PowerUpPlacement.typeReplace:
                AuthoringTool.onMapMutationRegionShift += SetMiniMap;
                break;
        }
    }

    public void Init()
    {
        _tileMapView = GetComponent<TileMapView>();
        _map = new MiniTileMap();
        _map.Init();
        _map.PaintTiles(_tileMapView.gridRect.transform,0.1f);
        _mapView = GetComponent<MiniMapView>();
    }

    public void ApplyToTileMapMain()
    {
        if (_map.GetTileMap() != null)
        {
            tileMapMain.SetTileMap(_map.GetTileMap());
            _mapView.Btn.SetActive(false);
        }
        onMiniMapApply?.Invoke();
        EventManagerUI.onMapReadyForPrediction?.Invoke();
    }

    public void SetMiniMap(List<KeyValuePair<TileMap, float>> balancedMaps)
    {
        _map.SetTileMap(balancedMaps[_index].Key.GetTileMap());
        _map.Render();
        var percent = balancedMaps[_index].Value;
        SetMiniMapView(percent);
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                _map.FormatTileOrientation(i, j, TileEnums.EnviromentTiles.ground);
            }
        }
        Destroy(MapSuggestionMng.tempView);
    }

    public void SetMiniMapView(float percent)
    {
        _newBlueAmount = (1 - percent) * 100;
        _newRedAmount = percent * 100;
        var curBlueAmount = (1 - currKillRatio) * 100;
        var curRedAmount = currKillRatio * 100;

        var blueSign = (curBlueAmount - _newBlueAmount) < 0 ? "-" : "+";
        var redSign = (curRedAmount - _newRedAmount) < 0 ? "-" : "+";

        //_mapView.SetKillRatioBar(_newBlueAmount / 100.0f, _newRedAmount / 100.0f);
        MetricsManager.SetKillRatioBar(_newBlueAmount / 100.0f, _newRedAmount / 100.0f, _mapView.KillRatioBar);
        //use three whitespaces for better alignment.
        _mapView.BluePercentText.text = $"{_newBlueAmount.ToString($"F0")} %";
        _mapView.RedPercentText.text = $"{_newRedAmount.ToString($"F0")} %";

        var result = MetricsManager.CalculateRatioDifference(percent, currKillRatio);
        if (result < 0)
        {
            _mapView.ResultsText.text = $"+{Mathf.Abs(result).ToString($"F0")} % balance gain";
            _mapView.ResultsText.color = Color.green;
        }
        else
        {
            _mapView.ResultsText.text = $"-{result.ToString($"F0")} % balance loss";
            _mapView.ResultsText.color = Color.red;
        }

        SetPowerUpsCount();

        //set button active
        if (!_mapView.Btn.activeInHierarchy)
        {
            _mapView.Btn.SetActive(true);
        }
    }

    private void SetPowerUpsCount()
    {

        var healthCount = _map.GetDecoration(TileEnums.Decorations.healthPack).Count;
        var healthDiff = healthCount - tileMapMain.GetDecoration(TileEnums.Decorations.healthPack).Count;
        // IMPORTANT there is an error and the values of armor and damage boost are switched.
        // Therefore, until error is solved, the texts will have switched values.
        // This occurs only on the minimap. Error propably in MapSuggestionManager.cs
        var armorCount = _map.GetDecoration(TileEnums.Decorations.armorVest).Count;
        var dmgCount = _map.GetDecoration(TileEnums.Decorations.damageBoost).Count;
        var armorDiff = armorCount - tileMapMain.GetDecoration(TileEnums.Decorations.armorVest).Count;
        var dmgDiff = dmgCount - tileMapMain.GetDecoration(TileEnums.Decorations.damageBoost).Count;


        _mapView.HealthCountText.text = $"{healthCount.ToString()} ({healthDiff.ToString("+0;-#")})";
        _mapView.ArmorCountText.text = $"{armorCount.ToString()} ({armorDiff.ToString("+0;-#")})";
        _mapView.DmgCountText.text = $"{dmgCount.ToString()} ({dmgDiff.ToString("+0;-#")})";
    }
}
