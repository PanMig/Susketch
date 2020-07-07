using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserLogsBuilder : MonoBehaviour
{
    [SerializeField] private bool _logging;
    [SerializeField] private MiniMap _adjustedMap;
    [SerializeField] private MiniMap _replacedMap;

    void Start()
    {
        UserLogsWriter.SetUniqueId();
        var dir = Application.dataPath + "/Collected Analytics";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (_logging)
        {
            EventManagerUI.onSingleClickEdit += CollectMainCanvasData;
            AuthoringTool.onPredictionsEnded += CollectPredictionsData;
            AuthoringTool.onSuggestionsEnded += CollectSuggestionsData;
        }
    }

    public void CollectMainCanvasData()
    {
        var core = new CoreMapClassPair(
            AuthoringTool.tileMapMain.ExportToStringArray(),
            CharacterClassMng.Instance.BlueClass.className,
            CharacterClassMng.Instance.RedClass.className,
            AuthoringTool.tileMapMain.GetDecorationsCount(),
            AuthoringTool._mapPlayable);

        if(AuthoringTool._activeTab == Enums.UIScreens.MapProperties)
        {
            var pathLog = new PathFindingLog(
                PathManager.Instance.pathTarget,
                PathManager.Instance.pathBlueProps.movementSteps,
                PathManager.Instance.pathRedProps.movementSteps);

            UserLogsWriter.LogMapProperties(core, pathLog, Enums.UIScreens.MapProperties.ToString());
        }
    }

    public void CollectPredictionsData()
    {
        var core = new CoreMapClassPair(
            AuthoringTool.tileMapMain.ExportToStringArray(),
            CharacterClassMng.Instance.BlueClass.className,
            CharacterClassMng.Instance.RedClass.className,
            AuthoringTool.tileMapMain.GetDecorationsCount(),
            AuthoringTool._mapPlayable);

        if (AuthoringTool._activeTab == Enums.UIScreens.Predictions)
        {
            var predictionLog = new PredictionsLog(
                AuthoringTool.currKillRatio,
                AuthoringTool.currDuration,
                AuthoringTool.currDArc,
                AuthoringTool.currCombatPace,
                AuthoringTool.currHeatmap);

            UserLogsWriter.LogPredictions(core, predictionLog, Enums.UIScreens.Predictions.ToString());
        }
    }

    public void CollectSuggestionsData()
    {
        var core = new CoreMapClassPair(
            AuthoringTool.tileMapMain.ExportToStringArray(),
            CharacterClassMng.Instance.BlueClass.className,
            CharacterClassMng.Instance.RedClass.className,
            AuthoringTool.tileMapMain.GetDecorationsCount(),
            AuthoringTool._mapPlayable);

        if (AuthoringTool._activeTab == Enums.UIScreens.Suggestions)
        {
            var adjucements = new SuggestionLog(_adjustedMap.Map.ExportToStringArray(), _adjustedMap.KillRatio,
                _adjustedMap.PercentageError, _adjustedMap.Map.GetDecorationsCount());

            var replacements = new SuggestionLog(_replacedMap.Map.ExportToStringArray(), _replacedMap.KillRatio,
                _replacedMap.PercentageError, _replacedMap.Map.GetDecorationsCount());

            var suggestions = new SuggestionLog[2] { adjucements, replacements };

            UserLogsWriter.LogSuggestions(core, suggestions, Enums.UIScreens.Suggestions.ToString());
        }
    }
}