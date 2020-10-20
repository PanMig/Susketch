using System.IO;
using UnityEngine;
using static UserLogsWriter;

public class UserLogsBuilder : MonoBehaviour
{
    [SerializeField] private bool _logging;
    [SerializeField] private MiniMap _adjustedMap;
    [SerializeField] private MiniMap _replacedMap;
    [SerializeField] private ClassBalanceView _distinct;
    [SerializeField] private ClassBalanceView _equal;
    private CoreMapClassPair mainCanvas;

    void Start()
    {
        if (_logging)
        {
            EventManagerUI.onMapReadyForPrediction += CollectMainCanvasData;
            AuthoringTool.onPredictionsEnded += CollectPredictionsData;
            AuthoringTool.onSuggestionsEnded += CollectCalculatedSuggestions;
            MiniMap.onMiniMapApply += CollectCalculatedSuggestions;
            ClassBalanceView.onClassBalanceApplied += CollectCalculatedSuggestions;
        }
    }

    public static void InitFileNameSettings(Enums.TutorialSessions sess)
    {
        SetUniqueId();
        SetDateTime();
        LogsDir = Application.dataPath + $"/Collected Analytics/{sess.ToString()}__{Date}";
        SetLogDirectory();
        // create a main directory to save user sessions.
        if (!Directory.Exists(LogsDir))
        {
            Directory.CreateDirectory(LogsDir);
        }
    }

    public void CollectMainCanvasData()
    {
        mainCanvas = new CoreMapClassPair(AuthoringTool.tileMapMain.ExportToStringArray(),
        CharacterClassMng.Instance.BlueClass.className,
        CharacterClassMng.Instance.RedClass.className,
        AuthoringTool.tileMapMain.GetDecorationsCount(),
        AuthoringTool._mapPlayable);

        var pathLog = new PathFindingLog(PathManager.Instance.pathTarget,
            PathManager.Instance.pathBlueProps.movementSteps,
            PathManager.Instance.pathRedProps.movementSteps);

        LogMapProperties(mainCanvas, pathLog);
    }

    public void CollectPredictionsData()
    {
        if (AuthoringTool._activeTab == Enums.UIScreens.Predictions && AuthoringTool._mapPlayable)
        {
            var predictionLog = new PredictionsLog(
                AuthoringTool.currKillRatio,
                AuthoringTool.currDuration,
                AuthoringTool.currDArc,
                AuthoringTool.currCombatPace,
                AuthoringTool.currHeatmap);
            
            LogPredictions(mainCanvas, predictionLog);
        }
    }

    public void CollectCalculatedSuggestions()
    {
        if (AuthoringTool._activeTab == Enums.UIScreens.Suggestions && AuthoringTool._mapPlayable)
        {
            var adjucements = new MapSuggestionLog(_adjustedMap.Map.ExportToStringArray(), _adjustedMap.KillRatio,
                _adjustedMap.PercentageError, _adjustedMap.Map.GetDecorationsCount(), _adjustedMap.Applied);
            var replacements = new MapSuggestionLog(_replacedMap.Map.ExportToStringArray(), _replacedMap.KillRatio,
                _replacedMap.PercentageError, _replacedMap.Map.GetDecorationsCount(), _replacedMap.Applied);

            var suggestions = new MapSuggestionLog[2] { adjucements, replacements };

            var distinct = new ClassSuggestionLog(_distinct.PercentageError, _distinct.KillRatio, _distinct.ClassesText, _distinct.wasApplied);
            var similar = new ClassSuggestionLog(_equal.PercentageError, _equal.KillRatio, _equal.ClassesText, _equal.wasApplied);

            var classes = new ClassSuggestionLog[2]{distinct, similar};

            LogSuggestions(mainCanvas, suggestions, classes);
        }
    }
}