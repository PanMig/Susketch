using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileOccurences : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthCount;
    [SerializeField] private TextMeshProUGUI _armorCount;
    [SerializeField] private TextMeshProUGUI _dmgCount;
    [SerializeField] private TextMeshProUGUI _stairsCount;
    [SerializeField] private TextMeshProUGUI _groundCount;
    [SerializeField] private TextMeshProUGUI _level1Count;
    [SerializeField] private TextMeshProUGUI _level2Count;

    public void OnEnable()
    {
        AuthoringTool.onMapInitEnded += SetTextValues;
        EventManagerUI.onTileMapEdit += SetTextValues;
        MiniMap.onMiniMapApply += SetTextValues;
    }

    public void OnDisable()
    {
        AuthoringTool.onMapInitEnded -= SetTextValues;
        EventManagerUI.onTileMapEdit -= SetTextValues;
        MiniMap.onMiniMapApply -= SetTextValues;
    }

    public void SetTextValues()
    {
        _healthCount.text = AuthoringTool.tileMapMain.GetDecoration(TileEnums.Decorations.healthPack).Count.ToString();
        _armorCount.text = AuthoringTool.tileMapMain.GetDecoration(TileEnums.Decorations.armorVest).Count.ToString();
        _dmgCount.text = AuthoringTool.tileMapMain.GetDecoration(TileEnums.Decorations.damageBoost).Count.ToString();
        _stairsCount.text = AuthoringTool.tileMapMain.GetDecoration(TileEnums.Decorations.stairs).Count.ToString();

        _groundCount.text = AuthoringTool.tileMapMain.GetEnviromentTiles(TileEnums.EnviromentTiles.ground).Count.ToString();
        _level1Count.text = AuthoringTool.tileMapMain.GetEnviromentTiles(TileEnums.EnviromentTiles.level_1).Count.ToString();
        _level2Count.text = AuthoringTool.tileMapMain.GetEnviromentTiles(TileEnums.EnviromentTiles.level_2).Count.ToString();
    }
}
