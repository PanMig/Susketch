using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;
using Random = System.Random;

public class RegionSwap : IPowerupPlacement
{
    private const int GENERATIONS = 10;
    private const float THRESHOLD = 0.5f;

    public Task<List<KeyValuePair<Tile[,], float>>> ChangePowerUps(TileMap tilemapMain)
    {
        MapSuggestionMng.tempView = new GameObject("TempView");
        // create a deep copy of the main tileMap.
        var tempMap = new TileMap();
        tempMap.Init();
        tempMap.InitRegions();
        tempMap.PaintTiles(MapSuggestionMng.tempView.transform, 1.0f);
        tempMap.SetTileMap(tilemapMain.GetTileMap());

        var task = Task.Run(() =>
        {
            // dictionary to keep the generated maps and scores
            Dictionary<Tile[,], float> mapsDict = new Dictionary<Tile[,], float>(new TileMapComparer());

            //Get a 2D array with all valid for placement ground and first level tiles.
            var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);
            var weaponsInput = GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass);

            for (int m = 0; m < GENERATIONS; m++)
            {
                var map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                map = ChangePickUpsRegion(map, placementLocations, tempMap.GetDecorations());
                var score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                if (!mapsDict.ContainsKey(map))
                {
                    mapsDict.Add(map, score);
                }
            }

            var balancedMaps = (from pair in mapsDict
                orderby Math.Abs(pair.Value - THRESHOLD)
                select pair).ToList();
            return balancedMaps;
        });
        return task;
    }

    public static Tile[,] ChangePickUpsRegion(Tile[,] map, List<Tile>[,] validLocations, Dictionary<string, List<Tile>> pickups)
    {
        foreach (var pickup in pickups)
        {
            string key = pickup.Key;
            foreach (var value in pickup.Value)
            {
                var regionNum = value.GetRegion();
                var randomRegion = MapSuggestionMng.GetRandomRegionWithNoPowerUps(regionNum.Item1, regionNum.Item2, validLocations, map);
                // no empty region found
                if (randomRegion.Item1 == -1)
                {
                    continue;
                }
                // get a random tile in the randomly selected region.
                if (validLocations[randomRegion.Item1, randomRegion.Item2].Count > 0)
                {
                    var randomIdx = MapSuggestionMng.RNG.Next(0, validLocations[randomRegion.Item1, randomRegion.Item2].Count);
                    var randomTile = validLocations[randomRegion.Item1, randomRegion.Item2][randomIdx];

                    if (randomTile.X != value.X && randomTile.Y != value.Y)
                    {
                        switch (key)
                        {
                            case "healthPack":
                                map[randomTile.X, randomTile.Y].decID = TileEnums.Decorations.healthPack;
                                map[value.X, (int)value.Y].decID = TileEnums.Decorations.empty;
                                break;
                            case "armorVest":
                                map[randomTile.X, randomTile.Y].decID = TileEnums.Decorations.armorVest;
                                map[value.X, (int)value.Y].decID = TileEnums.Decorations.empty;
                                break;
                            case "damageBoost":
                                map[randomTile.X, randomTile.Y].decID = TileEnums.Decorations.damageBoost;
                                map[value.X, (int)value.Y].decID = TileEnums.Decorations.empty;
                                break;
                        }
                    }
                }
            }
        }
        return map;
    }

    private static void PlaceNewRemoveOld(TileMap map, Tile randomTile, Tile value, TileEnums.Decorations decType)
    {
        var newTile = map.GetTileWithIndex(randomTile.X, randomTile.Y);
        var oldTile = map.GetTileWithIndex(value.X,value.Y);
        newTile.decID = decType;
        oldTile.decID = TileEnums.Decorations.empty;
        map.SetTileMapTile(newTile);
        map.SetTileMapTile(oldTile);
    }
}
