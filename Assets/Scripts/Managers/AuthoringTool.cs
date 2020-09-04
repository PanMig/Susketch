using UnityEngine;
using NumSharp;
using TileMapLogic;
using static MapSuggestionMng;
using static TFModel;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using Undo_Mechanism;
using UnityEditor;

// Change it with the TEMPLATE METHOD pattern.
public class AuthoringTool : MonoBehaviour
{
    // Managers
    public MetricsManager metricsMng;
    // Tilemap
    public static TileMapView tileMapViewMain;
    public static TileMap tileMapMain;
    private Tile[,] generatedMap = null;
    // Network input
    private NDArray input_map;
    private NDArray input_weapons;
    //Events
    public delegate void OnMapInitEnded();
    public static event OnMapInitEnded onMapInitEnded;

    public delegate void OnMapLoaded();
    public static event OnMapLoaded onMapLoaded;

    public delegate void OnPredictionsEnded();
    public static event OnPredictionsEnded onPredictionsEnded;

    public delegate void OnSuggestionsEnded();
    public static event OnSuggestionsEnded onSuggestionsEnded;

    public delegate void OnMapSuggestionsReady(List<KeyValuePair<Tile[,], float>> balancedMaps);
    public static event OnMapSuggestionsReady onPowerupsReplacement;
    public static event OnMapSuggestionsReady onPowerupsAdjucement;

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
    public static bool _mapPlayable;
    public static Enums.UIScreens _activeTab;

    // Predictions
    public static float currKillRatio;
    public static float currDuration;
    public static float[] currDArc;
    public static float[] currCombatPace;
    public static float[,] currHeatmap;

    private void OnEnable()
    {
        //onTileMapEdit is fired when a tile or decoration is added to the map.
        EventManagerUI.onSingleClickEdit += PaintTeamRegions;
        EventManagerUI.onSingleClickEdit += CheckTileMapListener;
        
        //on undo or redo event
        UndoRedoHandler.onUndoRedoEvent += CheckTileMapListener;
        
        //onMapReadyForPrediction is fired on End of drag AND pointer up.
        EventManagerUI.onMapReadyForPrediction += InvokeSurrogateModels;
        // CharacterClassMng is fired when the class selector is edited.
        CharacterClassMng.onClassSelectorEdit += InvokeSurrogateModels;

        //when class balance is applied.
        ClassBalanceView.onClassBalanceApplied += InvokeSurrogateModels;

        // When minimap is applied.
        MiniMap.onMiniMapApply += InvokeSurrogateModels;
        MiniMap.onMiniMapApply += PaintTeamRegions;
    }

    private void OnDisable()
    {
        EventManagerUI.onSingleClickEdit -= PaintTeamRegions;
        EventManagerUI.onSingleClickEdit -= CheckTileMapListener;
        UndoRedoHandler.onUndoRedoEvent -= CheckTileMapListener;
        EventManagerUI.onMapReadyForPrediction -= InvokeSurrogateModels;
        CharacterClassMng.onClassSelectorEdit -= InvokeSurrogateModels;
        ClassBalanceView.onClassBalanceApplied -= InvokeSurrogateModels;
        MiniMap.onMiniMapApply -= InvokeSurrogateModels;
        MiniMap.onMiniMapApply -= PaintTeamRegions;
    }
    
    // To be made singleton

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        Instance = this;
    //    }
    //}

    // Start is called before the first frame update
    private void Start()
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
        _activeTab = Enums.UIScreens.MapProperties;

        Debug.developerConsoleVisible = false;
    }

    public void SetActiveTab(int index)
    {
        switch (index)
        {
            case (int)Enums.UIScreens.MapProperties:
                _activeTab = Enums.UIScreens.MapProperties;
                break;
            case (int)Enums.UIScreens.Predictions:
                _activeTab = Enums.UIScreens.Predictions;
                break;
            case (int)Enums.UIScreens.Suggestions:
                _activeTab = Enums.UIScreens.Suggestions;
                break;
            case (int)Enums.UIScreens.Outro:
                _activeTab = Enums.UIScreens.Outro;
                break;
        }
    }

    private static void PaintTeamRegions()
    {
        //tileMapMain.PaintRegion(0, 3, 0);
        //tileMapMain.PaintRegion(3, 0, 0);
    }

    public void CheckTileMapListener()
    {
        _mapPlayable = TileMapRepair.CheckTileMap(tileMapMain);
        if (_mapPlayable)
        {
            TileMapRepair.onPlayableMap?.Invoke();
            _loadingMapTaskBusy = false;
            return;
        }
        TileMapRepair.onUnPlayableMap?.Invoke();
        _loadingMapTaskBusy = false;
    }

    public void LoadPredifinedMap()
    {
        if (!_loadingMapTaskBusy)
        {
            _loadingMapTaskBusy = true;
            // create a temp parent to save all instantiated tiles
            GameObject tempView = new GameObject("TempView");
            var randomMap = new TileMap();
            randomMap.Init();
            randomMap.InitRegions();
            randomMap.PaintTiles(tempView.transform, 1.0f);

            if (mapIndex < PREDEFINED_MAPS)
            {
                mapIndex++;
                Debug.Log("map index: " + mapIndex);
            }
            else
            {
                mapIndex = 1;
            }

            randomMap.ReadCSVToTileMap($"custom_{mapIndex}.txt", true);

            tileMapMain.SetTileMap(randomMap.GetTileMap());
            SetTileOrientation();
            Destroy(tempView);
            CheckTileMapListener();
            //PaintTeamRegions();
            InvokeSurrogateModels();
            CalculateClassBalanceAsync();
            CalculateBalancedPickUpsAsync();
            onMapLoaded?.Invoke();
        }
    }

    public void LoadMap(string fileName)
    {
        if (!_loadingMapTaskBusy)
        {
            _loadingMapTaskBusy = true;
            // create a temp parent to save all instantiated tiles
            GameObject tempView = new GameObject("TempView");
            var randomMap = new TileMap();
            randomMap.Init();
            randomMap.InitRegions();
            randomMap.PaintTiles(tempView.transform, 1.0f);

            
            randomMap.ReadCSVToTileMap($"{fileName}.csv", false);
            
            tileMapMain.SetTileMap(randomMap.GetTileMap());
            SetTileOrientation();
            Destroy(tempView);
            CheckTileMapListener();
            //PaintTeamRegions();
            InvokeSurrogateModels();
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
        InvokeSurrogateModels();
        onMapLoaded?.Invoke();
    }

    public void InvokeSurrogateModels()
    {
        if (_mapPlayable)
        {
            // Predictions
            InvokePredictions();

            // Procedural suggestions
            InvokeSuggestions();
        }

    }

    public void InvokePredictions()
    {
        KillRatioListener();
        if (_activeTab == Enums.UIScreens.Predictions)
        {
            DeathHeatmapListenerSmall();
            DramaticArcListener();
            CombatPaceListener();
            GameDurationListener();
        }
    }

    public void InvokeSuggestions()
    {
        CalculateClassBalanceAsync();
        CalculateBalancedPickUpsAsync();
    }

    //TODO : Change this to one method or at least compute it for one time.

    public async void DeathHeatmapListenerOverlay()
    {
        SetModelInput();
        var results = await PredictDeathHeatmap(input_map, input_weapons);
        currHeatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
        metricsMng.DeathHeatmapButtonListener(currHeatmap);
        metricsMng.GenerateDeathHeatmap(currHeatmap);
    }

    public async void DeathHeatmapListenerSmall()
    {
        if (!_heatmapTaskBusy)
        {
            _heatmapTaskBusy = true;
            SetModelInput();
            var results = await PredictDeathHeatmap(input_map, input_weapons);
            currHeatmap = ArrayParsingUtils.Make2DArray(results, 4, 4);
            metricsMng.GenerateDeathHeatmap(currHeatmap);
            _heatmapTaskBusy = false;
        }
    }

    public async void DramaticArcListener()
    {
        if (!_daTaskBusy)
        {
            _daTaskBusy = true;
            SetModelInput();
            currDArc = await PredictDramaticArc(input_map, input_weapons);
            metricsMng.GenerateDramaticArcGraph(currDArc);
            _daTaskBusy = false;
        }
    }

    public async void CombatPaceListener()
    {
        if(!_cpTaskBusy)
        {
            _cpTaskBusy = true;
            SetModelInput();
            currCombatPace = await PredictCombatPace(input_map, input_weapons);
            for (int i = 0; i < currCombatPace.Length; i++)
            {
                currCombatPace[i] = currCombatPace[i] * 20.0f;
            }

            metricsMng.GenerateCombatPaceGraph(currCombatPace);
            _cpTaskBusy = false;
        }
    }

    public async void KillRatioListener()
    {
        if (!_krTaskBusy)
        {
            _krTaskBusy = true;
            SetModelInput();
            //result returns the kills of player one (red) divided by the total kills.
            currKillRatio = await PredictKillRatio(input_map, input_weapons);
            metricsMng.SetKillRatioProgressBar(currKillRatio);
            _krTaskBusy = false;
        }
    }

    public async void GameDurationListener()
    {
        if (!_durationTaskBusy)
        {
            _durationTaskBusy = true;
            SetModelInput();
            currDuration = await PredictGameDuration(input_map, input_weapons);
            metricsMng.SetGameDurationText(currDuration);
            _durationTaskBusy = false;

            // quick fix as it is the last awaited call.
            onPredictionsEnded?.Invoke();
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
        if (!MapSuggestionMng.classBalanceTaskBusy)
        {
            KillRatioListener();
            onCharactersBalanced?.Invoke(false);
            var balanced_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.distinctMatches, GetInputMap(tileMapMain));
            onclassBalanceDistinct?.Invoke(balanced_classes);
            var same_classes = await GetBalancedMatchUpAsynchronous(FPSClasses.EqualMatches, GetInputMap(tileMapMain));
            onclassBalanceSame?.Invoke(same_classes);
            onCharactersBalanced?.Invoke(true);
        }
    }

    public async void CalculateBalancedPickUpsAsync()
    {
        if (!MapSuggestionMng.pickUpsTaskBusy)
        {
            KillRatioListener();
            var replacements = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.randomReplacement);
            onPowerupsReplacement?.Invoke(replacements);
            var adjustments = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.randomMutation);
            onPowerupsAdjucement?.Invoke(adjustments);
            _loadingMapTaskBusy = false;
            onSuggestionsEnded?.Invoke();
        }
    }
}
