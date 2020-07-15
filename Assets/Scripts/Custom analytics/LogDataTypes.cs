﻿using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LogDataTypes
{

}

public struct MapPropertiesTabLog
{
    public CoreMapClassPair MapClassPair;
    public PathFindingLog PathFindingLog;

    public MapPropertiesTabLog(CoreMapClassPair mapClassPair, PathFindingLog pathFindingLog)
    {
        this.MapClassPair = mapClassPair;
        this.PathFindingLog = pathFindingLog;
    }
}

public struct PredictionsTabLog
{
    public CoreMapClassPair MapClassPair;
    public PredictionsLog Predictions;

    public PredictionsTabLog(CoreMapClassPair mapClassPair, PredictionsLog predictions)
    {
        this.MapClassPair = mapClassPair;
        this.Predictions = predictions;
    }
}

public struct SuggestionsTabLog
{
    public CoreMapClassPair MapClassPair;
    public MapSuggestionLog[]  Suggestions;
    public ClassSuggestionLog[] classSuggestions;

    public SuggestionsTabLog(CoreMapClassPair mapClassPair, MapSuggestionLog[] mapSuggestions, ClassSuggestionLog[] classSuggestions)
    {
        this.MapClassPair = mapClassPair;
        this.Suggestions = mapSuggestions;
        this.classSuggestions = classSuggestions;
    }
}

public struct MapSuggestionLog
{
    public float PercentageError;
    public float SuggestedKR;
    public string[,] SuggestedMap;
    public Dictionary<string, int> PowerUps;
    public bool wasApplied;

    public MapSuggestionLog(string[,] suggestedMap, float suggestedKr, float percentageError, Dictionary<string, int> powerUps, bool wasApplied)
    {
        this.SuggestedMap = suggestedMap;
        this.SuggestedKR = suggestedKr;
        this.PercentageError = percentageError;
        this.PowerUps = powerUps;
        this.wasApplied = wasApplied;
    }
}

public struct ClassSuggestionLog
{
    public float PercentageError;
    public float SuggestedKR;
    public string SuggestedClasses;
    public bool wasApplied;

    public ClassSuggestionLog(float percentageError, float suggestedKr, string suggestedClasses, bool wasApplied)
    {
        PercentageError = percentageError;
        SuggestedKR = suggestedKr;
        SuggestedClasses = suggestedClasses;
        this.wasApplied = wasApplied;
    }
}

public struct CoreMapClassPair
{
    public string[,] Map;
    public string BlueClass;
    public string RedClass;
    public bool Playable;
    public Dictionary<string, int> Powerups;

    public CoreMapClassPair(string[,] map, string blueClass, string redClass, Dictionary<string,int> powerUps, bool playable)
    {
        this.Map = map;
        this.BlueClass = blueClass;
        this.RedClass = redClass;
        this.Playable = playable;
        this.Powerups = powerUps;
    }
}

public struct PredictionsLog
{
    public float KillRatio;
    public float Duration;
    public float[] DArc;
    public float[] CombatPace;
    public float[,] Heatmap;

    public PredictionsLog(float killRatio, float duration, float[] dArc, float[] combatPace, float[,] heatmap)
    {
        this.KillRatio = killRatio;
        this.Duration = duration;
        this.DArc = dArc;
        this.CombatPace = combatPace;
        this.Heatmap = heatmap;
    }
}

public struct PathFindingLog
{
    public int powerUpTarget;
    public int movementStepsBlue;
    public int movementStepsRed;

    public PathFindingLog(int powerUpTarget, int movementStepsBlue, int movementStepsRed)
    {
        this.powerUpTarget = powerUpTarget;
        this.movementStepsBlue = movementStepsBlue;
        this.movementStepsRed = movementStepsRed;
    }
}



