using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using NumSharp;
using static TileMapLogic.TileMap;
using static MLSuggestionsMng;
using static TFModel;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private FPSClasses fpsClasses;
    public MetricsManager metricsMng;
    public MLSuggestionsMng suggestionsMng;
    public TileMapView tileMapView;

    private NDArray input_map;
    private NDArray input_weapons;

    public Dropdown playerBlueDropdown;
    public Dropdown playerRedDropdown;
    public Text arc_text;


    // Start is called before the first frame update
    void Start()
    {
        fpsClasses = GetComponentInChildren<FPSClasses>();
        InitTileMap(tileMapView.gridRect.transform);
        InitRegions();
        PaintRegion(3, 0, 4);
        PaintRegion(0, 3, 5);

        //InvokeRepeating("FindClassBalance", 2.0f, 5.0f);
    }

    //private NDArray GetMapInput()
    //{
    //    var map = GetTileMapToString();
    //    input_map = ArrayParsingUtils.ParseToChannelArray(map);
    //    input_map = np.expand_dims(input_map, 0);
    //    return input_map;
    //}

    // TODO : Make sure that team0 (red) is first in the array and then team1(blue).
    private NDArray GetWeaponInputs()
    {
        var teamBlue = new CharacterClass(fpsClasses.characters[playerBlueDropdown.value].class_params);
        var teamRed = new CharacterClass(fpsClasses.characters[playerRedDropdown.value].class_params);

        var blue = teamBlue.Class_params;
        var red = teamRed.Class_params;

        // concat two arrays (first red then blue)
        var merged = new double[blue.Length + red.Length];
        red.CopyTo(merged, 0);
        blue.CopyTo(merged, blue.Length);

        NDArray arr = new NDArray(merged);
        input_weapons = np.array(arr);

        input_weapons = np.expand_dims(input_weapons, 0);
        return input_weapons;
    }

    public void DeathHeatmapButtonHandler()
    {
        SetModelInput();
        var results = PredictDeathHeatmap(input_map, input_weapons);
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.GenerateDeathHeatmap(heatmap);
    }

    public void DramaticArcButtonHandler()
    {
        SetModelInput();
        var results = PredictDramaticArc(input_map, input_weapons);
        arc_text.text = results.ToString();
        if (results > 0) { arc_text.color = Color.blue; }
        else { arc_text.color = Color.red; }
    }

    private void SetModelInput()
    {
        input_map = GetInputMap();
        input_weapons = GetInputWeapons(fpsClasses.characters[playerBlueDropdown.value], 
            fpsClasses.characters[playerRedDropdown.value]);
    }

    public void KillRatioButtonHandler()
    {
        SetModelInput();
        input_map = ConcatCoverChannel(input_map);
        var results = PredictKillRatio(input_map, input_weapons);
        Debug.Log(results);
    }

    public async void FindClassBalance()
    {
        var balanced_classes = await GetBalancedMatchup(fpsClasses.matchups, GetInputMap());
        Debug.Log(balanced_classes[0] + ", " + balanced_classes[1]);
    }

    public async void GeneratePickUps()
    {

    }

}
