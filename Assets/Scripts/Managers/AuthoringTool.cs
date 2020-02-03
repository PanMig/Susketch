﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using NumSharp;
using TileMapLogic;
using static MapSuggestionMng;
using static TFModel;
using Michsky.UI.ModernUIPack;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    private FPSClasses fpsClasses;
    public MetricsManager metricsMng;
    public MapSuggestionMng suggestionsMng;
    public static TileMapView tileMapView;
    public static TileMap tileMapMain;

    private NDArray input_map;
    private NDArray input_weapons;

    public HorizontalSelector blueSelector;
    public HorizontalSelector redSelector;
    public static CharacterParams blueClass;
    public static CharacterParams redClass;
    public Text arc_text;
    private CharacterParams[] balanced_classes;
    private Tile[,] generatedMap;

    private void OnEnable()
    {
        EventManagerUI.onTileMapEdit += PaintTeamRegions;
        EventManagerUI.onTileMapEdit += CheckTileMap;
        EventManagerUI.onTileMapEdit += CalculateBalancedPickUpsAsync;
    }

    private void OnDisable()
    {
        EventManagerUI.onTileMapEdit -= PaintTeamRegions;
        EventManagerUI.onTileMapEdit -= CheckTileMap;
        EventManagerUI.onTileMapEdit -= CalculateBalancedPickUpsAsync;

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
        PaintTeamRegions();
        CheckTileMap();
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
    }

    private void Invokes()
    {
        InvokeRepeating("OnDeathHeatmap", 1.0f, 60.0f);
        InvokeRepeating("DramaticArcButtonHandler", 1.0f, 60.0f);
        InvokeRepeating("KillRatioButtonHandler", 1.0f, 60.0f);
    }

    private void Update()
    {
        //tileMapMain.PaintRegion(3, 0, 4);
        //tileMapMain.PaintRegion(0, 3, 5);
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

    public void GameDurationButtonHandler()
    {
        SetModelInput();
        var results = PredictKillRatio(input_map, input_weapons);
        metricsMng.SetGameDurationText(results);
    }

    private void SetModelInput()
    {
        input_map = GetInputMap(tileMapMain);
        input_weapons = GetInputWeapons(fpsClasses.characters[blueSelector.index], 
            fpsClasses.characters[redSelector.index]);
        Debug.Log("Got input");
    }

    public async void CalculateClassBalanceAsync()
    {
        //balanced_classes = await GetBalancedMatchUpAsynchronous(fpsClasses.matchups, GetInputMap(tileMapMain));
        balanced_classes = await GetBalancedMatchUpAsynchronous(fpsClasses.matchups, GetInputMap(tileMapMain));
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if(x == false)
        {
            generatedMap = await SpawnPickupsAsynchronous(tileMapMain);
        }
        else
        {
            Debug.Log("wait for thread");
        }
    }

    public void FindClassBalance()
    {
        redClass = balanced_classes[0];
        blueClass = balanced_classes[1];
        Debug.Log($"blue: {blueClass}" + $"red: {redClass}");
        blueSelector.index = 3;
        blueSelector.index = 0;
    }

    public void GeneratePickUps()
    {
        tileMapMain.SetTileMap(generatedMap);
        tileMapMain.RenderTileMap();
    }
}
