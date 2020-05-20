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
    //Events
    public delegate void OnMapInitEnded();
    public static event OnMapInitEnded onMapInitEnded;

    public delegate void OnMapSuggestionsReady(List<KeyValuePair<TileMap,float>> balancedMaps);
    public static event OnMapSuggestionsReady onMapSuggestionsReady;

    public delegate void OnClassBalanceDistinct(KeyValuePair<CharacterParams[],float> balancedMatch);
    public static event OnClassBalanceDistinct onclassBalanceDistinct;
    public static event OnClassBalanceDistinct onclassBalanceSame;

    // Task shedulers
    public bool  heatmapTaskBusy = false;
    private bool daTaskBusy;
    private bool cpTaskBusy;
    private bool krTaskBusy;
    private bool durationTaskBusy;


    private void OnEnable()
    {
        //onTileMapEdit is fired when a tile or decoration is added to the map.
        EventManagerUI.onTileMapEdit += PaintTeamRegions;
        EventManagerUI.onTileMapEdit += CheckTileMapListener;
        
        //onMapReadyForPrediction is fired on End of drag and pointer up.
        EventManagerUI.onMapReadyForPrediction += InvokeMetrics;
        EventManagerUI.onMapReadyForPrediction += CalculateBalancedPickUpsAsync;
        EventManagerUI.onMapReadyForPrediction += CalculateClassBalanceAsync;

        // CharacterClassMng is fired when the class selector is edited.
        CharacterClassMng.onClassSelectorEdit += InvokeMetrics;
        CharacterClassMng.onClassSelectorEdit += CalculateClassBalanceAsync;

        // When minimap is applied.
        MiniMap.onMiniMapApply += InvokeMetrics;
        MiniMap.onMiniMapApply += PaintTeamRegions;
    }

    private void OnDisable()
    {
        EventManagerUI.onTileMapEdit -= PaintTeamRegions;
        EventManagerUI.onTileMapEdit -= CheckTileMapListener;
        EventManagerUI.onMapReadyForPrediction -= InvokeMetrics;
        EventManagerUI.onMapReadyForPrediction -= CalculateClassBalanceAsync;
        CharacterClassMng.onClassSelectorEdit -= InvokeMetrics;
        CharacterClassMng.onClassSelectorEdit -= CalculateBalancedPickUpsAsync;
        CharacterClassMng.onClassSelectorEdit -= CalculateClassBalanceAsync;
        MiniMap.onMiniMapApply -= InvokeMetrics;
        MiniMap.onMiniMapApply -= PaintTeamRegions;
    }

    // Start is called before the first frame update
    void Start()
    {
        tileMapMain = new TileMap();
        tileMapViewMain = GameObject.FindGameObjectWithTag("tileMapViewMain").GetComponent<TileMapView>();
        tileMapMain.PaintTiles(tileMapViewMain.gridRect.transform, 0.28f);
        tileMapMain.InitRegions();
        //PaintTeamRegions();
        SetTileOrientation();
        CheckTileMapListener();

        //Fire event for ready map.
        onMapInitEnded?.Invoke();
    }

    private static void PaintTeamRegions()
    {
        Color blueColor = new Color(255, 255, 255, 0.6f);
        //tileMapMain.PaintRegion(3, 0, blueColor);
        Color redColor = new Color(255, 255, 255, 0.6f);
        //tileMapMain.PaintRegion(0, 3, redColor);
        //tileMapMain.PaintRegion(3,0,0);
        //tileMapMain.PaintRegion(0,3,0);
        //tileMapMain.PaintRegionBorders(3, 0, 4);
        //tileMapMain.PaintRegionBorders(0, 3, 5);
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
        SetTileOrientation();
        randomMap = null;
        Destroy(tempView);
        CheckTileMapListener();
        PaintTeamRegions();
        InvokeMetrics();
    }

    public static void SetTileOrientation()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                tileMapMain.FormatTileOrientation(i, j, TileEnums.EnviromentTiles.ground);
            }
        }
    }

    public void EmptyMapListener()
    {
        tileMapMain.SetDefaultMap(0,0, tileMapViewMain.gridRect.transform);
        CheckTileMapListener();
        PaintTeamRegions();
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
            var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
            metricsMng.GenerateDeathHeatmap(heatmap);
            heatmapTaskBusy = false;
        }
    }

    public async void DramaticArcListener()
    {
        if (!daTaskBusy && TileMapPlayable())
        {
            daTaskBusy = true;
            SetModelInput();
            var results = await PredictDramaticArc(input_map, input_weapons);
            metricsMng.GenerateDramaticArcGraph(results);
            daTaskBusy = false;
        }
    }

    public async void CombatPaceListener()
    {
        if(!cpTaskBusy && TileMapPlayable())
        {
            cpTaskBusy = true;
            SetModelInput();
            var results = await PredictCombatPace(input_map, input_weapons);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = results[i] * 20.0f;
            }

            metricsMng.GenerateCombatPaceGraph(results);
            cpTaskBusy = false;
        }
    }

    public async void KillRatioListener()
    {
        if (!krTaskBusy && TileMapPlayable())
        {
            krTaskBusy = true;
            SetModelInput();
            //result returns the kills of player one (red) divided by the total kills.
            var results = await PredictKillRatio(input_map, input_weapons);
            currKillRatio = results;
            metricsMng.SetKillRatioProgressBar(results);
            krTaskBusy = false;
        }
    }

    public async void GameDurationListener()
    {
        if (!durationTaskBusy && TileMapPlayable())
        {
            durationTaskBusy = true;
            SetModelInput();
            var results = await PredictGameDuration(input_map, input_weapons);
            currDuration = results;
            metricsMng.SetGameDurationText(results);
            durationTaskBusy = false;
        }
    }

    private void SetModelInput()
    {
        input_map = GetInputMap(tileMapMain);
        // red player is player 1.
        input_weapons = GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass);
    }

    public async void CalculateClassBalanceAsync()
    {
        if (!MapSuggestionMng.classBalanceTaskBusy && TileMapPlayable())
        {
            var balanced_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.distinctMatches, GetInputMap(tileMapMain));
            onclassBalanceDistinct?.Invoke(balanced_classes);
            var same_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.EqualMatches, GetInputMap(tileMapMain));
            onclassBalanceSame?.Invoke(same_classes);
            KillRatioListener();
        }
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if (!MapSuggestionMng.pickUpsTaskBusy && TileMapPlayable())
        {
            var generatedMaps = await SpawnPickupsAsynchronous(tileMapMain);
            onMapSuggestionsReady?.Invoke(generatedMaps);
        }
    }
}
