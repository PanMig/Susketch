using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserLogsBuilder : MonoBehaviour
{
    [SerializeField] private bool _logging;
    [SerializeField] private MiniMap _adjustedMap;
    [SerializeField] private MiniMap _replacedMap;
    private CoreMapClassPair mainCanvas;

    void Start()
    {
        InitFileNameSettings();
        var dir = Application.dataPath + "/Collected Analytics";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (_logging)
        {
            EventManagerUI.onMapReadyForPrediction += CollectMainCanvasData;
            AuthoringTool.onPredictionsEnded += CollectPredictionsData;
            AuthoringTool.onSuggestionsEnded += CollectCalculatedSuggestions;
            MiniMap.onMiniMapApply += CollectAppliedSuggestions;
        }
    }

    public void InitFileNameSettings()
    {
        UserLogsWriter.SetUniqueId();
        UserLogsWriter.SetDateTime();
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

        UserLogsWriter.LogMapProperties(mainCanvas, pathLog);
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

            UserLogsWriter.LogPredictions(mainCanvas, predictionLog);
        }
    }

    public void CollectCalculatedSuggestions()
    {
        if (AuthoringTool._activeTab == Enums.UIScreens.Suggestions && AuthoringTool._mapPlayable)
        {
            var adjucements = new SuggestionLog(_adjustedMap.Map.ExportToStringArray(), _adjustedMap.KillRatio,
                _adjustedMap.PercentageError * -1, _adjustedMap.Map.GetDecorationsCount(), false);

            var replacements = new SuggestionLog(_replacedMap.Map.ExportToStringArray(), _replacedMap.KillRatio,
                _replacedMap.PercentageError * -1, _replacedMap.Map.GetDecorationsCount(), false);

            var suggestions = new SuggestionLog[2] { adjucements, replacements };

            UserLogsWriter.LogSuggestions(mainCanvas, suggestions);
        }
    }

    public void CollectAppliedSuggestions()
    {
        if (AuthoringTool._activeTab == Enums.UIScreens.Suggestions && AuthoringTool._mapPlayable)
        {
            var adjucements = new SuggestionLog(_adjustedMap.Map.ExportToStringArray(), _adjustedMap.KillRatio,
                _adjustedMap.PercentageError * -1, _adjustedMap.Map.GetDecorationsCount(), true);

            var replacements = new SuggestionLog(_replacedMap.Map.ExportToStringArray(), _replacedMap.KillRatio,
                _replacedMap.PercentageError * -1, _replacedMap.Map.GetDecorationsCount(), true);

            var suggestions = new SuggestionLog[2] { adjucements, replacements };

            UserLogsWriter.LogSuggestions(mainCanvas, suggestions);
        }
    }
}