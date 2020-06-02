﻿using UnityEngine;
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

    public delegate void OnMapLoaded();
    public static event OnMapLoaded onMapLoaded;

    public delegate void OnMapSuggestionsReady(List<KeyValuePair<TileMap,float>> balancedMaps);
    public static event OnMapSuggestionsReady onMapMutationRandom;
    public static event OnMapSuggestionsReady onMapMutationRegionShift;

    public delegate void OnClassBalanceDistinct(KeyValuePair<CharacterParams[],float> balancedMatch);
    public static event OnClassBalanceDistinct onclassBalanceDistinct;
    public static event OnClassBalanceDistinct onclassBalanceSame;

    // Task shedulers
    public bool  _heatmapTaskBusy;
    private bool _daTaskBusy;
    private bool _cpTaskBusy;
    private bool _krTaskBusy;
    private bool _durationTaskBusy;
    private static bool _loadingMapTaskBusy;

    // UI
    private int mapIndex = 0;
    private const int PREDEFINED_MAPS = 20;

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
        tileMapMain.PaintTiles(tileMapViewMain.gridRect.transform, 0.6f);
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
            _loadingMapTaskBusy = false;
            return;
        }
        TileMapRepair.onUnPlayableMap?.Invoke();
        _loadingMapTaskBusy = false;
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
        if (!_loadingMapTaskBusy)
        {
            _loadingMapTaskBusy = true;
            // create a temp parent to save all instantiated tiles
            GameObject tempView = new GameObject("TempView");
            var randomMap = new TileMap();
            randomMap.Init();
            randomMap.PaintTiles(tempView.transform, 1.0f);

            if (mapIndex <= PREDEFINED_MAPS)
            {
                mapIndex++;
                Debug.Log("map index: " + mapIndex);
            }
            else
            {
                mapIndex = 1;
            }
            randomMap.ReadCSVToTileMap($"Daniel files/custom_{mapIndex}");
            tileMapMain.SetTileMap(randomMap.GetTileMap());
            SetTileOrientation();
            Destroy(tempView);
            CheckTileMapListener();
            //PaintTeamRegions();
            InvokeMetrics();
            CalculateClassBalanceAsync();
            CalculateBalancedPickUpsAsync();
            onMapLoaded?.Invoke();
        }
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
        SetTileOrientation();
        CheckTileMapListener();
        PaintTeamRegions();
        InvokeMetrics();
        CalculateClassBalanceAsync();
        CalculateBalancedPickUpsAsync();
        onMapLoaded?.Invoke();
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
        if (!_heatmapTaskBusy && TileMapPlayable())
        {
            _heatmapTaskBusy = true;
            SetModelInput();
            var results = await PredictDeathHeatmap(input_map, input_weapons);
            var heatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
            metricsMng.GenerateDeathHeatmap(heatmap);
            _heatmapTaskBusy = false;
        }
    }

    public async void DramaticArcListener()
    {
        if (!_daTaskBusy && TileMapPlayable())
        {
            _daTaskBusy = true;
            SetModelInput();
            var results = await PredictDramaticArc(input_map, input_weapons);
            metricsMng.GenerateDramaticArcGraph(results);
            _daTaskBusy = false;
        }
    }

    public async void CombatPaceListener()
    {
        if(!_cpTaskBusy && TileMapPlayable())
        {
            _cpTaskBusy = true;
            SetModelInput();
            var results = await PredictCombatPace(input_map, input_weapons);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = results[i] * 20.0f;
            }

            metricsMng.GenerateCombatPaceGraph(results);
            _cpTaskBusy = false;
        }
    }

    public async void KillRatioListener()
    {
        if (!_krTaskBusy && TileMapPlayable())
        {
            _krTaskBusy = true;
            SetModelInput();
            //result returns the kills of player one (red) divided by the total kills.
            var results = await PredictKillRatio(input_map, input_weapons);
            currKillRatio = results;
            metricsMng.SetKillRatioProgressBar(results);
            _krTaskBusy = false;
        }
    }

    public async void GameDurationListener()
    {
        if (!_durationTaskBusy && TileMapPlayable())
        {
            _durationTaskBusy = true;
            SetModelInput();
            var results = await PredictGameDuration(input_map, input_weapons);
            currDuration = results;
            metricsMng.SetGameDurationText(results);
            _durationTaskBusy = false;
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
            onCharactersBalanced?.Invoke(false);
            var balanced_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.distinctMatches, GetInputMap(tileMapMain));
            onclassBalanceDistinct?.Invoke(balanced_classes);
            var same_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.EqualMatches, GetInputMap(tileMapMain));
            onclassBalanceSame?.Invoke(same_classes);
            KillRatioListener();
            onCharactersBalanced?.Invoke(true);
        }
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if (!MapSuggestionMng.pickUpsTaskBusy && TileMapPlayable())
        {
            var randomMutation = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.random);
            onMapMutationRandom?.Invoke(randomMutation);
            var regionShift = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.regionShift);
            onMapMutationRegionShift?.Invoke(regionShift);
            _loadingMapTaskBusy = false;
        }
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }
}
