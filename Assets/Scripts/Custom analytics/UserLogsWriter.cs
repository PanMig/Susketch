using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

public static class UserLogsWriter
{
    private static readonly string pathMP = Application.dataPath + "/Collected Analytics/SuSketchUser_MapPropertiesLogs";
    private static readonly string pathPreds = Application.dataPath + "/Collected Analytics/SuSketchUser_PredictionsLogs";
    private static readonly string pathSugg = Application.dataPath + "/Collected Analytics/SuSketchUser_SuggestionsLogs";
    public static string uniqueId;

    public static void LogMapProperties(CoreMapClassPair mainCanvas, PathFindingLog pathFindingLog, string activeTab)
    {
        Debug.Log("collecting MP");
        MapPropertiesTabLog log = new MapPropertiesTabLog(mainCanvas, pathFindingLog, activeTab);
        SaveToJson(pathMP, log);
    }

    public static void LogPredictions(CoreMapClassPair mainCanvas, PredictionsLog predictions, string activeTab)
    {
        Debug.Log("collecting Predictions");
        PredictionsTabLog log = new PredictionsTabLog(mainCanvas, predictions, activeTab);
        SaveToJson(pathPreds, log);
    }

    public static void LogSuggestions(CoreMapClassPair mainCanvas, SuggestionLog[] suggestions, string activeTab)
    {
        Debug.Log("collecting Suggestions");
        SuggestionsTabLog log = new SuggestionsTabLog(mainCanvas, suggestions, activeTab);
        SaveToJson(pathSugg, log);
    }

    private static void SaveToJson<T>(string path, T log)
    {
        path = $"{path}_{uniqueId}.json";
        List<T> existingEntries;
        if (File.Exists(path))
        {
            var jsonData = System.IO.File.ReadAllText(path);
            existingEntries = JsonConvert.DeserializeObject<List<T>>(jsonData);
        }
        else
        {
            existingEntries = new List<T>();
        }

        existingEntries.Add(log);
        var jsonEntry = JsonConvert.SerializeObject(existingEntries, Formatting.Indented);
        File.WriteAllText(path, jsonEntry);
    }

    public static void SetUniqueId()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
            .Range(65, 26)
            .Select(e => ((char)e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(11)
            .ToList().ForEach(e => builder.Append(e));
        uniqueId = builder.ToString();
        //PlayerPrefs.SetString("uniqueID", uniqueId);
    }
}
