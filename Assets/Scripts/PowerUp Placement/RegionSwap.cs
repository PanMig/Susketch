using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;

public class RegionSwap : IPowerupPlacement
{
    private const int GENERATIONS = 6;

    public async Task<List<KeyValuePair<TileMap, float>>> ChangePowerUps(TileMap tilemapMain)
    {
        MapSuggestionMng.tempView = new GameObject("TempView");
        var tempMap = tilemapMain.GetTileMap();
        TileMap map;
        var maps = new Dictionary<TileMap, float>();
        //Get a 2D array with all valid for placement ground and first level tiles.
        var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);

        for (int m = 0; m < GENERATIONS; m++)
        {
            map = new TileMap();
            map.Init();
            map.InitRegions();
            map.PaintTiles(MapSuggestionMng.tempView.transform, 1.0f);
            map.SetTileMap(tempMap);
            map = await ChangePickUpsRegion(tilemapMain, placementLocations).ConfigureAwait(false);
            var score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
            maps.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var balancedMaps = (from pair in maps
            orderby Math.Abs(pair.Value - MapSuggestionMng.thresshold)
            select pair).ToList();
        return balancedMaps;
    }

    private static Task<TileMap> ChangePickUpsRegion(TileMap map, List<Tile>[,] validLocations)
    {
        MapSuggestionMng.pickUpsTask = Task.Run(() =>
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
                    var randomIdx = MapSuggestionMng.RNG.Next(0, validLocations[randomRegion.Item1, randomRegion.Item2].Count);
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

        return MapSuggestionMng.pickUpsTask;
    }
}
