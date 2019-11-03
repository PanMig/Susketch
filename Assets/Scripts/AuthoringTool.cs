using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NumSharp;
using static TileMapLogic.TileMap;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private FPSClasses fpsClasses;
    public TFModel model;
    public MetricsManager metricsMng;
    public TileMapView tileMapView;

    private NDArray input_map;
    private NDArray input_weapons;

    public Dropdown playerBlue;
    public Dropdown playerRed;
    public Text arc_text;

    // Start is called before the first frame update
    void Start()
    {
        fpsClasses = GetComponentInChildren<FPSClasses>();
        InitTileMap(tileMapView.gridRect.transform);
        InitRegions();
        PaintRegion(3, 0, 4);
        PaintRegion(0, 3, 5);

    }

    private NDArray GetMapInput()
    {
        var map = GetTileMapToString();
        input_map = ArrayParsingUtils.ParseToChannelArray(map);
        input_map = np.expand_dims(input_map, 0);
        return input_map;
    }

    private NDArray GetWeaponInputs()
    {
        var teamBlue = new CharacterClass(fpsClasses.characters[playerBlue.value].class_params);
        var teamRed = new CharacterClass(fpsClasses.characters[playerRed.value].class_params);

        var blue = teamBlue.Class_params;
        var red = teamRed.Class_params;

        // concat two arrays
        var merged = new double[blue.Length + red.Length];
        blue.CopyTo(merged, 0);
        red.CopyTo(merged, blue.Length);

        NDArray arr = new NDArray(merged);
        input_weapons = np.array(arr);

        input_weapons = np.expand_dims(input_weapons, 0);
        return input_weapons;
    }

    public void DeathHeatmapButtonHandler()
    {
        var input_map = GetMapInput();
        var input_weapons = GetWeaponInputs();
        var results = model.PredictDeathHeatmap(input_map, input_weapons);
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.GenerateDeathHeatmap(heatmap);
    }

    public void DramaticArcButtonHandler()
    {
        var input_map = GetMapInput();
        var input_weapons = GetWeaponInputs();
        var results = model.PredictDramaticArc(input_map, input_weapons);
        arc_text.text = results.ToString();
        if(results > 0) { arc_text.color = Color.red; }
        else { arc_text.color = Color.blue; }
    }

}
