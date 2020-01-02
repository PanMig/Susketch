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

    private void OnEnable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        fpsClasses = GetComponentInChildren<FPSClasses>();
        SetClassParams();

        tileMapMain = new TileMap();
        tileMapView = GameObject.FindGameObjectWithTag("tileMapView").GetComponent<TileMapView>();
        tileMapMain.InitTileMap(tileMapView.gridRect.transform);
        tileMapMain.InitRegions();

        //tileMapMain.PaintRegion(3, 0, 4);
        //tileMapMain.PaintRegion(0, 3, 5);
        //tileMapMain.PaintRegion(0, 0, 2);

        for (int i = 0; i < 20; i++)
        {
            tileMapMain.GetTileWithIndex(11, i).PaintTile(Brush.Instance.brushThemes[1], tileMapMain);
        }
        //for (int i = 10; i <15; i++)
        //{
        //    tileMapMain.GetTileWithIndex(7, i).PaintTile(Brush.Instance.brushThemes[1], tileMapMain);
        //}
        //for (int i = 0; i < 7; i++)
        //{
        //    tileMapMain.GetTileWithIndex(15, i).PaintTile(Brush.Instance.brushThemes[1], tileMapMain);
        //}
        //for (int i = 7; i < 14; i++)
        //{
        //    tileMapMain.GetTileWithIndex(4, i).PaintTile(Brush.Instance.brushThemes[1], tileMapMain);
        //}
        //for (int i = 0; i < 3; i++)
        //{
        //    tileMapMain.GetTileWithIndex(0, i).PaintTile(Brush.Instance.brushThemes[2], tileMapMain);
        //}
        //for (int i = 10; i < 14; i++)
        //{
        //    tileMapMain.GetTileWithIndex(10, i).PaintTile(Brush.Instance.brushThemes[2], tileMapMain);
        //}
        // Invoke methods
        Invokes();
        
    }

    private void tEST()
    {
        var x = TileMapRepair.CheckTileMap();
        Debug.Log(x);
    }

    private void Invokes()
    {
        //InvokeRepeating("OnDeathHeatmap", 1.0f, 10.0f);
        //InvokeRepeating("DramaticArcButtonHandler", 1.0f, 10.0f);
        //InvokeRepeating("KillRatioButtonHandler", 1.0f, 5.0f);
        InvokeRepeating("tEST", 1.0f, 2.0f);
    }

    private void Update()
    {
        tileMapMain.PaintRegion(3, 0, 4);
        tileMapMain.PaintRegion(0, 3, 5);
    }

    public void SetClassParams()
    {
        blueClass = fpsClasses.characters[blueSelector.index];
        redClass = fpsClasses.characters[redSelector.index];
    }

    public void DeathHeatmapButtonHandler()
    {
        SetModelInput();
        var results = PredictDeathHeatmap(input_map, input_weapons);
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.DeathHeatmapButtonListener(heatmap);
    }

    public void OnDeathHeatmap()
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
        metricsMng.GenerateDramaticArcGraph(results);
    }

    public void KillRatioButtonHandler()
    {
        SetModelInput();
        var results = PredictKillRatio(input_map, input_weapons);
        metricsMng.SetKillRatioProgressBar(results * 100);
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
        blueSelector.index = 3;
        blueSelector.index = 0;
    }

    public void GeneratePickUps()
    {
        var map = SpawnBalancedPickUps(tileMapMain);
        tileMapMain.SetTileMap(map);
        tileMapMain.RenderTileMap();
    }

}
