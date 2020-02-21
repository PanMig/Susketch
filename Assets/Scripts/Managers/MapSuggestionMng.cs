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
    public static Task<TileMap> pickUpsTask;
    public static bool pickUpsTaskBusy;

    private readonly static System.Random RNG = new System.Random();
    readonly static float thresshold = 0.5f;
    private static float score = 0.0f;
    private static GameObject tempView = new GameObject("TempView");

    #region events

    public delegate void OnCharactersBalanced(bool value);
    public static event OnCharactersBalanced onCharactersBalanced;

    public delegate void OnPickUpsGenerated(bool value);
    public static event OnPickUpsGenerated onPickUpsGenerated;

    #endregion

    // Old implementation with the Task System.
    public async static Task<CharacterParams[]> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        return await Task.Run(async () =>
         {
             var classes = new CharacterParams[2];
             var thresshold = 0.5f;
             float score = 0;
             var scores = new List<float>();
             foreach (var matchup in classMatchups)
             {
                 var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                 score = await PredictKillRatio(input_map, input_weapons);
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
        Debug.Log("Balanced classes ended");
        onCharactersBalanced?.Invoke(true);
        return characterClasses;
    }

    private static int GetClosestIdxToThresshold(float thresshold, List<float> scores)
    {
        var closest = scores.Aggregate((x, y) => Mathf.Abs(x - thresshold) < Mathf.Abs(y - thresshold) ? x : y);
        var resultIdx = scores.IndexOf(closest);
        return resultIdx;
    }

    public static async Task<Tile[,]> SpawnPickupsAsynchronous(TileMap tilemapMain)
    {

        Debug.Log("Spawn Pickups started");
        pickUpsTaskBusy = true;
        onPickUpsGenerated?.Invoke(false);
        var map = await SpawnBalancedPickUps(tilemapMain);
        Debug.Log("Spawn Pickups ended");
        onPickUpsGenerated?.Invoke(true);
        pickUpsTaskBusy = false;
        return map;
    }

    public static async Task<Tile[,]> SpawnBalancedPickUps(TileMap tilemapMain)
    {
        var tempMap = tilemapMain.GetTileMap();
        TileMap map;
        foreach (Transform child in tempView.transform)
        {
            Destroy(child.gameObject);
        }
        Dictionary<TileMap, float> mapsDict = new Dictionary<TileMap, float>();

        // randomly select a region to spawn a pickups
        for (int m = 0; m < 5; m++)
        {
            // this will erase all previous decorations on the main map.
            map = new TileMap();
            map.InitTileMap(tempView.transform);
            map.SetTileMap(tempMap, tempView.transform);
            map.RemoveDecorations();
            map = await SetPickUpsLocations(map, RNG).ConfigureAwait(false);
            score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(blueClass, redClass));
            if (TileMapRepair.HasAccesiblePowerUps(map))
            {
                mapsDict.Add(map, score);
            }
            await new WaitForEndOfFrame();
        }

        var valueslist = mapsDict.Values.ToList();
        float bestMatch = valueslist[GetClosestIdxToThresshold(thresshold, valueslist)];
        var balancedMap = mapsDict.FirstOrDefault(x => x.Value == bestMatch);
        tempView.SetActive(false);
        map = null;
        return balancedMap.Key.GetTileMap();
    }

    private static Task<TileMap> SetPickUpsLocations(TileMap map, System.Random RNG)
    {
        pickUpsTask = Task.Run(() =>
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
                        Tile fillTile = map.GetRandomRegionCell(i, j, RNG);
                        if (fillTile.envTileID != TileEnums.EnviromentTiles.level_2)
                        {
                            fillTile.decID = TileEnums.Decorations.healthPack;
                            map.SetTileMapTile(fillTile);
                        }
                    }
                    else if (diceRoll == (int)pickups.armor)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j, RNG);
                        if (fillTile.envTileID != TileEnums.EnviromentTiles.level_2)
                        {
                            fillTile.decID = TileEnums.Decorations.armorVest;
                            map.SetTileMapTile(fillTile);
                        }
                    }
                    else if (diceRoll == (int)pickups.damage)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j, RNG);
                        if (fillTile.envTileID != TileEnums.EnviromentTiles.level_2)
                        {
                            fillTile.decID = TileEnums.Decorations.damageBoost;
                            map.SetTileMapTile(fillTile);
                        }
                    }
                    else
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j, RNG);
                        if (fillTile.envTileID != TileEnums.EnviromentTiles.level_2)
                        {
                            fillTile.decID = TileEnums.Decorations.empty;
                            map.SetTileMapTile(fillTile);
                        }
                            
                    }
                }
            }
            return map;
        });
        return pickUpsTask;
    }
}
