using UnityEngine;
using NumSharp;
using TileMapLogic;
using static MapSuggestionMng;
using static TFModel;
using Michsky.UI.ModernUIPack;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    // Managers
    public MetricsManager metricsMng;
    public MapSuggestionMng suggestionsMng;
    // Tilemap
    public static TileMapView tileMapView;
    public static TileMap tileMapMain;
    private Tile[,] generatedMap;
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

    private void OnEnable()
    {
        //onTileMapEdit is fired when a tile or decoration is added to the map.
        EventManagerUI.onTileMapEdit += PaintTeamRegions;
        EventManagerUI.onTileMapEdit += CheckTileMap;
        //onMapReadyForPrediction is fired on End of drag and pointer up.
        EventManagerUI.onMapReadyForPrediction += InvokeMetrics;
        CharacterClassMng.onClassSelectorEdit += InvokeMetrics;
        //EventManagerUI.onTileMapEdit += CalculateBalancedPickUpsAsync;

    }

    private void OnDisable()
    {
        EventManagerUI.onTileMapEdit -= PaintTeamRegions;
        EventManagerUI.onTileMapEdit -= CheckTileMap;
        EventManagerUI.onMapReadyForPrediction -= InvokeMetrics;
        CharacterClassMng.onClassSelectorEdit -= InvokeMetrics;
        //EventManagerUI.onTileMapEdit -= CalculateBalancedPickUpsAsync;

    }

    // Start is called before the first frame update
    void Start()
    {
        tileMapMain = new TileMap();
        tileMapView = GameObject.FindGameObjectWithTag("tileMapView").GetComponent<TileMapView>();
        tileMapMain.InitTileMap(tileMapView.gridRect.transform);
        tileMapMain.InitRegions();
        PaintTeamRegions();
        DeathHeatmapListenerSmall();
        CheckTileMap();

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

    public static void CheckTileMap()
    {
        if (TileMapRepair.CheckTileMap(tileMapMain))
        {
            TileMapRepair.onPlayableMap?.Invoke();
            return;
        }
        TileMapRepair.onUnPlayableMap?.Invoke();
    }

    public void LoadMap()
    {
        // create a temp parent to save all instantiated tiles
        GameObject tempView = new GameObject("TempView");
        var randomMap = new TileMap();
        randomMap.InitTileMap(tempView.transform);

        int index = Random.Range(1, 10);
        randomMap.ReadCSVToTileMap("Map Files/mapFile" + index);
        tileMapMain.SetTileMap(randomMap.GetTileMap());
        randomMap = null;
        Destroy(tempView);
        tileMapMain.RenderTileMap();
        CheckTileMap();
    }

    public void EmptyMapListener()
    {
        tileMapMain.SetDefaultMap(0,0);
        CheckTileMap();
    }

    private void InvokeMetrics()
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
        SetModelInput();
        var results = await PredictDeathHeatmap(input_map, input_weapons);
        Debug.Log("Death heatmap prediction");
        var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.GenerateDeathHeatmap(heatmap);
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
        metricsMng.SetKillRatioProgressBar(results);
        Debug.Log("Kill ratio prediction");
    }

    public async void GameDurationListener()
    {
        SetModelInput();
        var results = await PredictGameDuration(input_map, input_weapons);
        metricsMng.SetGameDurationText(results);
        Debug.Log("Game duration prediction");
    }

    private void SetModelInput()
    {
        input_map = GetInputMap(tileMapMain);
        // red player is player 1.
        input_weapons = GetInputWeapons(blueClass, redClass);
        Debug.Log("Got input");
    }

    public async void CalculateClassBalanceAsync()
    {
        balanced_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.matchups, GetInputMap(tileMapMain));
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if(x == false)
        {
            generatedMap = await SpawnPickupsAsynchronous(tileMapMain);

        }
    }

    public void FindClassBalance()
    {
        redClass = balanced_classes[0];
        blueClass = balanced_classes[1];
        Debug.Log($"blue: {blueClass}" + $"red: {redClass}");
    }

    public void GeneratePickUps()
    {
        tileMapMain.RemoveDecorations();
        tileMapMain.SetTileMap(generatedMap);
        tileMapMain.RenderTileMap();
    }
}
