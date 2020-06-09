﻿using System.Collections;
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
    private enum pickups { none, health, armor, damage };
    public static Task<TileMap> pickUpsTask;
    public static bool pickUpsTaskBusy;
    public static bool classBalanceTaskBusy;
    public static GameObject tempView;

    private readonly static System.Random RNG = new System.Random();
    readonly static float thresshold = 0.5f;
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
        if (replaceType == Enums.PowerUpPlacement.random)
        {
            maps = await SpawnRandomPickUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.regionShift)
        {
            maps = await ChangePickUpsLocation(tilemapMain);
        }
        else
        {
            maps = await ChangePickUpsLocation(tilemapMain);
        }
        onPickUpsGenerated?.Invoke(true);
        pickUpsTaskBusy = false;
        return maps;
    }

    public static async Task<List<KeyValuePair<TileMap, float>>> SpawnRandomPickUps(TileMap tilemapMain)
    {
        //used for spawning the new tiles and delete them after the search process.
        tempView = new GameObject("TempView");
        var tempMap = tilemapMain.GetTileMap();
        Dictionary<TileMap, float> mapsDict = new Dictionary<TileMap, float>();

        //Get a 2D array with all valid for placement ground and first level tiles.
        var placementLocations = GetValidPlacementLocations(tilemapMain);

        for (int m = 0; m < 10; m++)
        {
            var map = new TileMap();
            map.Init();
            map.PaintTiles(tempView.transform, 1.0f);
            map.SetTileMap(tempMap);

            // this will erase all previous decorations on the main map.
            map.RemoveDecorations();
            map = await SetPickUpsLocations(map, placementLocations).ConfigureAwait(false);
            score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
            mapsDict.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var balancedMaps = (from pair in mapsDict
                            orderby Math.Abs(pair.Value - thresshold)
                            select pair).ToList();
        return balancedMaps;
    }

    private static List<Tile>[,] GetValidPlacementLocations(TileMap map)
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

    private static Task<TileMap> SetPickUpsLocations(TileMap map, List<Tile>[,] validLocations)
    {
        pickUpsTask = Task.Run(() =>
        {
            int powerUpRoll;
            //iterate regions.
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    if (validLocations[i, j].Count == 0)
                    {
                        continue;
                    }

                    powerUpRoll = RNG.Next(1, 5);
                    var randomIdx = RNG.Next(0, validLocations[i, j].Count);
                    Tile fillTile;

                    if (powerUpRoll == (int)pickups.health)
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.healthPack;
                    }
                    else if (powerUpRoll == (int)pickups.armor)
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.armorVest;
                    }
                    else if (powerUpRoll == (int)pickups.damage)
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.damageBoost;
                    }
                    else
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.empty;
                    }
                    map.SetTileMapTile(fillTile);
                }
            }
            return map;
        });
        return pickUpsTask;
    }

    public static async Task<List<KeyValuePair<TileMap, float>>> ChangePickUpsLocation(TileMap tilemapMain)
    {
        tempView = new GameObject("TempView");
        var tempMap = tilemapMain.GetTileMap();
        TileMap map;
        var maps = new Dictionary<TileMap, float>();
        //Get a 2D array with all valid for placement ground and first level tiles.
        var placementLocations = GetValidPlacementLocations(tilemapMain);

        for (int m = 0; m < 6; m++)
        {
            map = new TileMap();
            map.Init();
            map.InitRegions();
            map.PaintTiles(tempView.transform, 1.0f);
            map.SetTileMap(tempMap);
            map = await ChangePickUpsRegion(map, placementLocations).ConfigureAwait(false);
            score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
            maps.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var balancedMaps = (from pair in maps
                            orderby Math.Abs(pair.Value - thresshold)
                            select pair).ToList();
        return balancedMaps;
    }

    private static Task<TileMap> ChangePickUpsRegion(TileMap map, List<Tile>[,] validLocations)
    {
        pickUpsTask = Task.Run(() =>
        {
            var pickups = map.GetDecorations();

            foreach (var pickup in pickups)
            {
                string key = "";
                key = pickup.Key;
                foreach (var value in pickup.Value)
                {
                    var regionNum = map.GetTileRegion((int)value[0], (int)value[1]);
                    var randomRegion = map.GetRandomRegionWithNoPowerUps(regionNum.Item1, regionNum.Item2, validLocations);
                    // get a random tile in the randomly selected region.
                    var randomIdx = RNG.Next(0, validLocations[randomRegion.Item1, randomRegion.Item2].Count);
                    var randomTile = validLocations[randomRegion.Item1, randomRegion.Item2][randomIdx];

                    switch (key)
                    {
                        case "healthPack":
                            randomTile.decID = TileEnums.Decorations.healthPack;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                        case "armorVest":
                            randomTile.decID = TileEnums.Decorations.armorVest;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                        case "damageBoost":
                            randomTile.decID = TileEnums.Decorations.damageBoost;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                    }
                    map.SetTileMapTile(randomTile);
                }
            }
            return map;
        });

        return pickUpsTask;
    }

    private static Task<TileMap> ChangePickUpsType(TileMap map, System.Random RNG)
    {
        pickUpsTask = Task.Run(() =>
        {
            var pickups = map.GetDecorations();

            foreach (var pickup in pickups)
            {
                string key = "";
                key = pickup.Key;
                foreach (var value in pickup.Value)
                {
                    var tile = map.GetTileWithIndex((int)value.x, (int)value.y);

                    var randomGuess = RNG.Next(0, 3);
                    switch (randomGuess)
                    {
                        case 0:
                            tile.decID = TileEnums.Decorations.healthPack;
                            break;
                        case 1:
                            tile.decID = TileEnums.Decorations.armorVest;
                            break;
                        case 2:
                            tile.decID = TileEnums.Decorations.damageBoost;
                            break;
                    }
                }
            }

            return map;
        });

        return pickUpsTask;
    }

    #endregion

}
