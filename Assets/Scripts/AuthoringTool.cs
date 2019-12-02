using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using NumSharp;
using TileMapLogic;
using static MLSuggestionsMng;
using static TFModel;
using Michsky.UI.ModernUIPack;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private FPSClasses fpsClasses;
    public MetricsManager metricsMng;
    public MLSuggestionsMng suggestionsMng;
    public static TileMapView tileMapView;
    public static TileMap tileMapMain;

    private NDArray input_map;
    private NDArray input_weapons;

    public HorizontalSelector blueSelector;
    public HorizontalSelector redSelector;
    public static CharacterParams blueClass;
    public static CharacterParams redClass;
    public Text arc_text;


    // Start is called before the first frame update
    void Start()
    {
        fpsClasses = GetComponentInChildren<FPSClasses>();
        SetClassParams();

        tileMapMain = new TileMap();
        tileMapView = GameObject.FindGameObjectWithTag("tileMapView").GetComponent<TileMapView>();
        tileMapMain.InitTileMap(tileMapView.gridRect.transform);
        tileMapMain.InitRegions();
        tileMapMain.PaintRegion(3, 0, 4);
        tileMapMain.PaintRegion(0, 3, 5);

        //InvokeRepeating("FindClassBalance", 2.0f, 5.0f);
        //Invoke("GeneratePickUps", 6.0f);
    }

    private void Update()
    {
        
    }

    public void SetClassParams()
    {
        blueClass = fpsClasses.characters[blueSelector.index];
        redClass = fpsClasses.characters[redSelector.index];
    }

    public void DeathHeatmapButtonHandler()
    {
        Debug.Log("death heatmap");
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

    public void KillRatioButtonHandler()
    {
        SetModelInput();
        var results = PredictKillRatio(input_map, input_weapons);
        Debug.Log(results);
    }

    private void SetModelInput()
    {
        input_map = GetInputMap(tileMapMain);
        input_weapons = GetInputWeapons(fpsClasses.characters[blueSelector.index], 
            fpsClasses.characters[redSelector.index]);
        Debug.Log("Got input");
    }

    public async void FindClassBalance()
    {
        var balanced_classes = await GetBalancedMatchup(fpsClasses.matchups, GetInputMap(tileMapMain));
        redClass = balanced_classes[0];
        blueClass = balanced_classes[1];
        Debug.Log("team red: " + redClass.name);
        Debug.Log("team blue: " + blueClass.name);
    }

    public void GeneratePickUps()
    {
        var map = SpawnBalancedPickUps(tileMapMain);
        tileMapMain.SetTileMap(map);
        tileMapMain.RenderTileMap();
    }

}
