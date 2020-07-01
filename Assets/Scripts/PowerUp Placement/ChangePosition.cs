using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using static TFModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChangePosition : IPowerupPlacement
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

            for (int m = 0; m < GENERATIONS; m++)
            {
                var map = ChangePickUpsPosInRegion(tempMap, placementLocations);
                var score = PredictKillRatioSynchronous(GetInputMap(map),
                    GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
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

    public static Tile[,] ChangePickUpsPosInRegion(TileMap map, List<Tile>[,] validLocations)
    {
        var pickups = map.GetDecorations();

        foreach (var pickup in pickups)
        {
            System.Random RNG = new System.Random();

            string key = pickup.Key;
            foreach (var value in pickup.Value)
            {
                var cur_region = map.GetTileRegion((int)value[0], (int)value[1]);
                if (cur_region.Item1 == -1)
                {
                    continue;
                }
                // get a random tile in the randomly selected region.
                if (validLocations[cur_region.Item1, cur_region.Item2].Count > 0)
                {
                    var randomIdx = RNG.Next(0, validLocations[cur_region.Item1, cur_region.Item2].Count);
                    var randomTile = validLocations[cur_region.Item1, cur_region.Item2][randomIdx];

                    switch (key)
                    {
                        case "healthPack":
                            map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.healthPack;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                        case "armorVest":
                            map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.armorVest;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                        case "damageBoost":
                            map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.damageBoost;
                            map.GetTileWithIndex((int)value[0], (int)value[1]).decID = TileEnums.Decorations.empty;
                            break;
                    }
                }
            }
        }
        return map.GetTileMap();
    }
}
