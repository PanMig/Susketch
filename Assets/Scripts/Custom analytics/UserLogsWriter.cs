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
    // user log storing paths
    private static string pathMP;
    private static string pathPreds;
    private static string pathSugg; 

    // file properties
    public static string UniqueId;
    public static string DateTime;
    public static string LogsDir;

    public static void LogMapProperties(CoreMapClassPair mainCanvas, PathFindingLog pathFindingLog)
    {
        //Debug.Log("collecting MainCanvas");
        MapPropertiesTabLog log = new MapPropertiesTabLog(mainCanvas, pathFindingLog);
        SaveToJson(pathMP, log);
    }

    public static void LogPredictions(CoreMapClassPair mainCanvas, PredictionsLog predictions)
    {
        //Debug.Log("collecting Predictions");
        PredictionsTabLog log = new PredictionsTabLog(mainCanvas, predictions);
        SaveToJson(pathPreds, log);
    }

    public static void LogSuggestions(CoreMapClassPair mainCanvas, MapSuggestionLog[] maps, ClassSuggestionLog[] classes)
    {
        //Debug.Log("collecting Suggestions");
        SuggestionsTabLog log = new SuggestionsTabLog(mainCanvas, maps, classes);
        SaveToJson(pathSugg, log);
    }

    #region JsonUtillities

    private static void SaveToJson<T>(string path, T log)
    {
        path = $"{path}_{DateTime}.json";
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

    #endregion

    #region Writer FILE settings

    public static void SetUniqueId()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
            .Range(65, 26)
            .Select(e => ((char) e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char) e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(11)
            .ToList().ForEach(e => builder.Append(e));
        UniqueId = builder.ToString();
        //PlayerPrefs.SetString("uniqueID", uniqueId);
    }

    public static void SetDateTime()
    {
        DateTime = System.DateTime.Now.ToString();
        DateTime = DateTime.Replace(" ", "_");
        DateTime = DateTime.Replace(":", "_");
        DateTime = DateTime.Replace("/", "_");
        DateTime = DateTime.Replace(".", "");
        DateTime = DateTime.Replace("μμ", "MM");
        DateTime = DateTime.Replace("πμ", "PM");
    }

    public static void SetLogDirectory()
    {
        pathMP = $"{LogsDir}/SSK_MainCanvasLogs";
        pathPreds  = $"{LogsDir}/SSK_PredictionsLogs";
        pathSugg = $"{LogsDir}/SSK_SuggestionsLogs";
    }

    #endregion

    
}
