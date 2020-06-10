using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;

public class RandomReplacement : IPowerupPlacement
{

    private const int GENERATIONS = 10;
    private const float THRESHOLD = 0.5f;

    public async Task<List<KeyValuePair<TileMap, float>>> ChangePowerUps(TileMap tilemapMain)
    {
        //used for spawning the new tiles and delete them after the search process.
        MapSuggestionMng.tempView = new GameObject("TempView");
        var tempMap = tilemapMain.GetTileMap();
        Dictionary<TileMap, float> mapsDict = new Dictionary<TileMap, float>();

        //Get a 2D array with all valid for placement ground and first level tiles.
        var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);

        for (int m = 0; m < GENERATIONS; m++)
        {
            var map = new TileMap();
            map.Init();
            map.PaintTiles(MapSuggestionMng.tempView.transform, 1.0f);
            map.SetTileMap(tempMap);

            // this will erase all previous decorations on the main map.
            map.RemoveDecorations();
            map = await SetPickUpsLocations(map, placementLocations).ConfigureAwait(false);
            var score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
            mapsDict.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var balancedMaps = (from pair in mapsDict
            orderby Math.Abs(pair.Value - THRESHOLD)
            select pair).ToList();
        return balancedMaps;
    }

    private Task<TileMap> SetPickUpsLocations(TileMap map, List<Tile>[,] validLocations)
    {
        MapSuggestionMng.pickUpsTask = Task.Run(() =>
        {
            int powerUpRoll;
            //iterate regions.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (validLocations[i, j].Count == 0)
                    {
                        continue;
                    }

                    powerUpRoll = MapSuggestionMng.RNG.Next(1, 5);
                    var randomIdx = MapSuggestionMng.RNG.Next(0, validLocations[i, j].Count);
                    Tile fillTile;

                    if (powerUpRoll == (int)MapSuggestionMng.pickups.health)
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.healthPack;
                    }
                    else if (powerUpRoll == (int)MapSuggestionMng.pickups.armor)
                    {
                        fillTile = validLocations[i, j][randomIdx];
                        fillTile.decID = TileEnums.Decorations.armorVest;
                    }
                    else if (powerUpRoll == (int)MapSuggestionMng.pickups.damage)
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
        return MapSuggestionMng.pickUpsTask;
    }
}
