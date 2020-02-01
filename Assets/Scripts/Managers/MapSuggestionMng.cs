using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NumSharp;
using System;
using System.Threading.Tasks;
using static TFModel;
using TileMapLogic;
using static AuthoringTool;
using UnityEngine.UI;

public class MapSuggestionMng : MonoBehaviour
{
    private enum pickups { none, health, armor, damage };
    public static bool x;

    public delegate void OnCharactersBalanced();
    public static event OnCharactersBalanced OnBalancedCharacters;

    public delegate void OnPickUpsGenerated();
    public static event OnPickUpsGenerated OnGeneratedPickUps;


    // Old implementation with the Task System.
    public static Task<CharacterParams[]> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        return Task.Run(() =>
        {
            var classes = new CharacterParams[2];
            var thresshold = 0.5f;
            float score = 0;
            var scores = new List<float>();
            foreach (var matchup in classMatchups)
            {
                var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                score = TFModel.PredictKillRatio(input_map, input_weapons);
                scores.Add(score);
            }

            int resultIdx = GetClosestIdxToThresshold(thresshold, scores);

            classes[0] = classMatchups[resultIdx][0]; // red team.
            classes[1] = classMatchups[resultIdx][1]; // blue team.

            return classes;
        });
    }

    public static async Task<CharacterParams[]> GetBalancedMatchUpAsynchronous(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        Debug.Log("Balanced classes started");
        var characterClasses = await GetBalancedMatchup(classMatchups, input_map).ConfigureAwait(false);
        OnBalancedCharacters?.Invoke();
        Debug.Log("Balanced classes ended");
        return characterClasses;
    }

    public static async Task<CharacterParams[]> CalculateClassBalance(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        var classes = new CharacterParams[2];
        var thresshold = 0.5f;
        float score = 0;
        var scores = new List<float>();
        foreach (var matchup in classMatchups)
        {
            var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
            score = TFModel.PredictKillRatio(input_map, input_weapons);
            scores.Add(score);
            await new WaitForEndOfFrame();
        }

        int resultIdx = GetClosestIdxToThresshold(thresshold, scores);

        classes[0] = classMatchups[resultIdx][0]; // red team.
        classes[1] = classMatchups[resultIdx][1]; // blue team.
        OnBalancedCharacters?.Invoke();
        return classes;
    }

    private static int GetClosestIdxToThresshold(float thresshold, List<float> scores)
    {
        var closest = scores.Aggregate((x, y) => Mathf.Abs(x - thresshold) < Mathf.Abs(y - thresshold) ? x : y);
        var resultIdx = scores.IndexOf(closest);
        return resultIdx;
    }

    public static async Task<Tile[,]> SpawnPickupsAsynchronous(TileMap tilemapMain)
    {
        if (OnGeneratedPickUps.GetInvocationList().Length == 1)
        {
            x = true;
            Debug.Log("Spawn Pickups started");
            var map = await SpawnBalancedPickUps(tilemapMain);
            OnGeneratedPickUps?.Invoke();
            Debug.Log("Spawn Pickups ended");
            x = false;
            return map;
        }
        return tileMapMain.GetTileMap();
    }

    // TODO : More than one tile are spawned inside the region.
    public static async Task<Tile[,]> SpawnBalancedPickUps(TileMap tilemapMain)
    {
        TileMap map;
        GameObject tempView = new GameObject("TempView");
        Dictionary<TileMap, float> mapsDict = new Dictionary<TileMap, float>();
        float thresshold = 0.5f;
        float score = 0.0f;
        int diceRoll;
        System.Random RNG = new System.Random();

        // randomly select a region to spawn a pickups
        for (int m = 0; m < 40; m++)
        {
            // this will erase all previous decorations on the main map.
            map = new TileMap();
            map.InitTileMap(tempView.transform);
            map.SetTileMap(tilemapMain.GetTileMap());
            map.RemoveDecorations();
            map = await SetPickUpsLocations(map, RNG).ConfigureAwait(false);
            score = PredictKillRatio(GetInputMap(map), GetInputWeapons(blueClass, redClass));
            mapsDict.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var valueslist = mapsDict.Values.ToList();
        var bestMatch = valueslist[GetClosestIdxToThresshold(thresshold, valueslist)];
        var balancedMap = mapsDict.FirstOrDefault(x => x.Value == bestMatch);
        Destroy(tempView);
        return balancedMap.Key.GetTileMap();

    }

    private static Task<TileMap> SetPickUpsLocations(TileMap map, System.Random RNG)
    {
        return Task.Run(() =>
        {
            int diceRoll;
            //iterate regions.
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    diceRoll = RNG.Next(1, 5);
                    if (diceRoll == (int)pickups.health)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.healthPack;
                        map.SetTileMapTile(fillTile);
                    }
                    else if (diceRoll == (int)pickups.armor)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.armorVest;
                        map.SetTileMapTile(fillTile);
                    }
                    else if (diceRoll == (int)pickups.damage)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.damageBoost;
                        map.SetTileMapTile(fillTile);
                    }
                    else
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.empty;
                        map.SetTileMapTile(fillTile);
                    }
                }
            }
            return map;
        });
    }
}
