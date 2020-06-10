using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NumSharp;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using static TFModel;
using TileMapLogic;
using static AuthoringTool;
using UnityEngine.UI;

public class MapSuggestionMng : MonoBehaviour
{
    public enum pickups { none, health, armor, damage };
    public static Task<TileMap> pickUpsTask;
    public static bool pickUpsTaskBusy;
    public static bool classBalanceTaskBusy;
    public static GameObject tempView;

    public static System.Random RNG = new System.Random();
    public static float thresshold = 0.5f;
    private static float score = 0.0f;

    #region events

    public delegate void OnCharactersBalanced(bool value);
    public static OnCharactersBalanced onCharactersBalanced;

    public delegate void OnPickUpsGenerated(bool value);
    public static event OnPickUpsGenerated onPickUpsGenerated;

    #endregion

    #region Class Balance

    public static Task<KeyValuePair<CharacterParams[], float>> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray inputMap)
    {
        return Task.Run(async () =>
        {
            var classes = new CharacterParams[2];
            var thresshold = 0.5f;
            float score = 0;
            var scores = new List<float>();
            foreach (var matchup in classMatchups)
            {
                var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                score = await PredictKillRatio(inputMap, input_weapons);
                scores.Add(score);
            }

            int resultIdx = GetClosestIdxToThresshold(thresshold, scores);

            classes[0] = classMatchups[resultIdx][0]; // red team.
            classes[1] = classMatchups[resultIdx][1]; // blue team.
            var classPair = new KeyValuePair<CharacterParams[], float>(classes, scores[resultIdx]);
            return classPair;
        });
    }

    public static async Task<KeyValuePair<CharacterParams[], float>> GetBalancedMatchUpAsynchronous(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        classBalanceTaskBusy = true;
        var characterClasses = await GetBalancedMatchup(classMatchups, input_map).ConfigureAwait(false);
        classBalanceTaskBusy = false;
        return characterClasses;
    }

    private static int GetClosestIdxToThresshold(float thresshold, List<float> scores)
    {
        var closest = scores.Aggregate((x, y) => Mathf.Abs(x - thresshold) < Mathf.Abs(y - thresshold) ? x : y);
        var resultIdx = scores.IndexOf(closest);
        return resultIdx;
    }

    #endregion

    #region Pickups Placement

    public static async Task<List<KeyValuePair<TileMap, float>>> SpawnPickupsAsynchronous(TileMap tilemapMain, Enums.PowerUpPlacement replaceType)
    {
        pickUpsTaskBusy = true;
        onPickUpsGenerated?.Invoke(false);
        List<KeyValuePair<TileMap, float>> maps;
        if (replaceType == Enums.PowerUpPlacement.randomReplacement)
        {
            var rndReplacement = new RandomReplacement();
            maps = await rndReplacement.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.regionSwap)
        {
            var regSwap = new RegionSwap();
            maps = await regSwap.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.typeAlteration)
        {
            var typeAlter = new TypeAlteration();
            maps = await typeAlter.ChangePowerUps(tilemapMain);
        }
        else
        {
            var posShift = new PositionShift();
            maps = await posShift.ChangePowerUps(tilemapMain);
        }
        onPickUpsGenerated?.Invoke(true);
        pickUpsTaskBusy = false;
        return maps;
    }

    public static List<Tile>[,] GetValidPlacementLocations(TileMap map)
    {
        const int regionRows = 4, regionCols = 4;
        List<Tile>[,] validLocations = new List<Tile>[regionRows, regionCols];

        for (int i = 0; i < regionRows; i++)
        {
            for (int j = 0; j < regionCols; j++)
            {
                validLocations[i, j] = new List<Tile>();
                var groundTiles = map.Regions[i, j].GetEnvironmentalTiles(TileEnums.EnviromentTiles.ground);
                var floorTiles = map.Regions[i, j].GetEnvironmentalTiles(TileEnums.EnviromentTiles.level_1);
                if (groundTiles.Count > 0)
                {
                    validLocations[i, j].AddRange(groundTiles);
                }
                if (floorTiles.Count > 0)
                {
                    validLocations[i, j].AddRange(floorTiles);
                }
            }
        }
        return validLocations;
    }


    #endregion

}
