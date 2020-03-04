using UnityEngine;
using NumSharp;
using TileMapLogic;
using static MapSuggestionMng;
using static TFModel;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    // Managers
    public MetricsManager metricsMng;
    public MapSuggestionMng suggestionsMng;
    // Tilemap
    public static TileMapView tileMapViewMain;
    public static TileMap tileMapMain;
    private Tile[,] generatedMap = null;
    public static float currKillRatio;
    public static float currDuration;
    // Network input
    private NDArray input_map;
    private NDArray input_weapons;
    // Character classes
    public static CharacterParams blueClass;
    public static CharacterParams redClass;
    private CharacterParams[] balanced_classes;
    //Events
    public delegate void OnMapInitEnded();
    public static event OnMapInitEnded onMapInitEnded;

    public delegate void OnMapSuggestionsReady(List<KeyValuePair<TileMap,float>> balancedMaps);
    public static event OnMapSuggestionsReady onMapSuggestionsReady;
    // Task shedulers
    public bool heatmapTaskBusy = false;


    private void OnEnable()
    {
        //onTileMapEdit is fired when a tile or decoration is added to the map.
        EventManagerUI.onTileMapEdit += PaintTeamRegions;
        EventManagerUI.onTileMapEdit += CheckTileMapListener;
        
        //onMapReadyForPrediction is fired on End of drag and pointer up.
        EventManagerUI.onMapReadyForPrediction += InvokeMetrics;
        EventManagerUI.onMapReadyForPrediction += CalculateBalancedPickUpsAsync;

        // CharacterClassMng is fired when the class selector is edited.
        CharacterClassMng.onClassSelectorEdit += InvokeMetrics;
    }

    private void OnDisable()
    {
        EventManagerUI.onTileMapEdit -= PaintTeamRegions;
        EventManagerUI.onTileMapEdit -= CheckTileMapListener;
        EventManagerUI.onMapReadyForPrediction -= InvokeMetrics;
        EventManagerUI.onMapReadyForPrediction -= CalculateBalancedPickUpsAsync;
        CharacterClassMng.onClassSelectorEdit -= InvokeMetrics;
    }

    // Start is called before the first frame update
    void Start()
    {
        tileMapMain = new TileMap();
        tileMapViewMain = GameObject.FindGameObjectWithTag("tileMapViewMain").GetComponent<TileMapView>();
        tileMapMain.PaintTiles(tileMapViewMain.gridRect.transform, 0.28f);
        tileMapMain.InitRegions();
        PaintTeamRegions();
        CheckTileMapListener();

        //Fire event for ready map.
        onMapInitEnded?.Invoke();
    }

    private static void PaintTeamRegions()
    {
        // Color the team regions
        Color blueColor = new Color(0, 0, 255, 0.6f);
        Color redColor = new Color(255, 0, 0, 0.6f);
        tileMapMain.PaintRegion(3, 0, blueColor);
        tileMapMain.PaintRegion(0, 3, redColor);
    }

    public static void CheckTileMapListener()
    {
        if (TileMapRepair.CheckTileMap(tileMapMain))
        {
            TileMapRepair.onPlayableMap?.Invoke();
            return;
        }
        TileMapRepair.onUnPlayableMap?.Invoke();
    }

    public bool TileMapPlayable()
    {
        if (TileMapRepair.CheckTileMap(tileMapMain))
        {
            return true;
        }
        return false;
    }

    public void LoadMap()
    {
        // create a temp parent to save all instantiated tiles
        GameObject tempView = new GameObject("TempView");
        var randomMap = new TileMap();
        randomMap.Init();
        randomMap.PaintTiles(tempView.transform,1.0f);

        int index = Random.Range(1, 10);
        randomMap.ReadCSVToTileMap("Map Files/mapFile" + index);
        tileMapMain.SetTileMap(randomMap.GetTileMap());
        randomMap = null;
        Destroy(tempView);
        CheckTileMapListener();
    }

    public void EmptyMapListener()
    {
        tileMapMain.SetDefaultMap(0,0, tileMapViewMain.gridRect.transform);
        CheckTileMapListener();
    }

    public void InvokeMetrics()
    {
        DeathHeatmapListenerSmall();
        DramaticArcListener();
        CombatPaceListener();
        KillRatioListener();
        GameDurationListener();
    }

    public async void DeathHeatmapListenerOverlay()
    {
        SetModelInput();
        var results = await PredictDeathHeatmap(input_map, input_weapons);
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.DeathHeatmapButtonListener(heatmap);
        metricsMng.GenerateDeathHeatmap(heatmap);
    }

    public async void DeathHeatmapListenerSmall()
    {
        if (!heatmapTaskBusy && TileMapPlayable())
        {
            heatmapTaskBusy = true;
            SetModelInput();
            var results = await PredictDeathHeatmap(input_map, input_weapons);
            Debug.Log("Death heatmap prediction");
            var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
            metricsMng.GenerateDeathHeatmap(heatmap);
            heatmapTaskBusy = false;
        }
    }

    public async void DramaticArcListener()
    {
        SetModelInput();
        var results = await PredictDramaticArc(input_map, input_weapons);
        Debug.Log("Dramatic arc prediction");
        metricsMng.GenerateDramaticArcGraph(results);
    }

    public async void CombatPaceListener()
    {
        SetModelInput();
        var results = await PredictCombatPace(input_map, input_weapons);
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = results[i] * 20.0f;
        }
        Debug.Log("Combat pace prediction");
        metricsMng.GenerateCombatPaceGraph(results);
    }

    public async void KillRatioListener()
    {
        SetModelInput();
        //result returns the kills of player one (red) divided by the total kills.
        var results = await PredictKillRatio(input_map, input_weapons);
        currKillRatio = results;
        metricsMng.SetKillRatioProgressBar(results);
        Debug.Log("Kill ratio prediction");
    }

    public async void GameDurationListener()
    {
        SetModelInput();
        var results = await PredictGameDuration(input_map, input_weapons);
        currDuration = results;
        metricsMng.SetGameDurationText(results);
        Debug.Log("Game duration prediction");
    }

    private void SetModelInput()
    {
        input_map = GetInputMap(tileMapMain);
        // red player is player 1.
        input_weapons = GetInputWeapons(blueClass, redClass);
    }

    public async void CalculateClassBalanceAsync()
    {
        balanced_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.matchups, GetInputMap(tileMapMain));
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if (!MapSuggestionMng.pickUpsTaskBusy && TileMapPlayable())
        {
            Debug.Log("Spawn pick ups called");
            var generatedMaps = await SpawnPickupsAsynchronous(tileMapMain);
            onMapSuggestionsReady?.Invoke(generatedMaps);
        }
    }

    public void FindClassBalance()
    {
        redClass = balanced_classes[0];
        blueClass = balanced_classes[1];
        Debug.Log($"blue: {blueClass}" + $"red: {redClass}");
    }
}
