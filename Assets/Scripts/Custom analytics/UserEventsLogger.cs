using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public static class UserEventsLogger
{

    public static void LogMainCanvas(string[,] mainMap, string blueClass, string redClass, string activeTab)
    {
        string path = Application.dataPath + "/Core.json";
        List<MainCanvasLog> existingEntries;
        MainCanvasLog log = new MainCanvasLog(mainMap, blueClass, redClass, activeTab);
        JsonSerializer serializer = new JsonSerializer();

        if (File.Exists(path))
        {
            var jsonData = System.IO.File.ReadAllText(path);
            existingEntries = JsonConvert.DeserializeObject<List<MainCanvasLog>>(jsonData);
        }
        else
        {
            existingEntries = new List<MainCanvasLog>();
        }

        existingEntries.Add(log);
        var jsonEntry = JsonConvert.SerializeObject(existingEntries, Formatting.Indented);
        File.WriteAllText(path, jsonEntry);


    }

    public static void LogPredictions(float kr, float duration , float[] cp, float[] da, float[][] heatmap)
    {

    }

    public static void LogMainMapProperties(string[][] mainMap, string[][] suggestedMap, string classPair)
    {

    }

    public static void LogSuggestions()
    {

    }
}
