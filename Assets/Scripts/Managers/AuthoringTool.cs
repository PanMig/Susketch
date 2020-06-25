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

    public delegate void OnMapLoaded();
    public static event OnMapLoaded onMapLoaded;

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
    public Enums.UIScreens _activeTab;

    private void OnEnable()
    {
        //onTileMapEdit is fired when a tile or decoration is added to the map.
        EventManagerUI.onSingleClickEdit += PaintTeamRegions;
        EventManagerUI.onSingleClickEdit += CheckTileMapListener;
        
        //onMapReadyForPrediction is fired on End of drag AND pointer up.
        EventManagerUI.onMapReadyForPrediction += InvokeSurrogateModels;

        // CharacterClassMng is fired when the class selector is edited.
        CharacterClassMng.onClassSelectorEdit += InvokeSurrogateModels;

        // When minimap is applied.
        MiniMap.onMiniMapApply += InvokeSurrogateModels;
        MiniMap.onMiniMapApply += PaintTeamRegions;
    }

    private void OnDisable()
    {
        EventManagerUI.onSingleClickEdit -= PaintTeamRegions;
        EventManagerUI.onSingleClickEdit -= CheckTileMapListener;
        EventManagerUI.onMapReadyForPrediction -= InvokeSurrogateModels;
        CharacterClassMng.onClassSelectorEdit -= InvokeSurrogateModels;
        MiniMap.onMiniMapApply -= InvokeSurrogateModels;
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
        _activeTab = Enums.UIScreens.MapProperties;
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
            randomMap.InitRegions();
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
        if (TileMapPlayable())
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
        if (!_heatmapTaskBusy)
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
        if (!_daTaskBusy)
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
        if(!_cpTaskBusy)
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
        if (!_krTaskBusy)
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
        if (!_durationTaskBusy)
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
        if (!MapSuggestionMng.classBalanceTaskBusy)
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
        if (!MapSuggestionMng.pickUpsTaskBusy)
        {
            var replacements = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.randomReplacement);
            onPowerupsReplacement?.Invoke(replacements);
            var adjustments = await SpawnPickupsAsynchronous(tileMapMain, Enums.PowerUpPlacement.regionSwap);
            onPowerupsAdjucement?.Invoke(adjustments);
            _loadingMapTaskBusy = false;
        }
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }
}
