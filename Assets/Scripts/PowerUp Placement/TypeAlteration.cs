using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;
using Random = System.Random;

public class TypeAlteration : IPowerupPlacement
{
    private const int GENERATIONS = 10;

    public async Task<List<KeyValuePair<TileMap, float>>> ChangePowerUps(TileMap tilemapMain)
    {
        MapSuggestionMng.tempView = new GameObject("TempView");
        var tempMap = tilemapMain.GetTileMap();
        TileMap map;
        var maps = new Dictionary<TileMap, float>();

        for (int m = 0; m < GENERATIONS; m++)
        {
            map = new TileMap();
            map.Init();
            map.InitRegions();
            map.PaintTiles(MapSuggestionMng.tempView.transform, 1.0f);
            map.SetTileMap(tempMap);
            map = await ChangePickUpsType(tilemapMain).ConfigureAwait(false);
            var score = await PredictKillRatio(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
            maps.Add(map, score);
            await new WaitForEndOfFrame();
        }

        var balancedMaps = (from pair in maps
            orderby Math.Abs(pair.Value - MapSuggestionMng.thresshold)
            select pair).ToList();
        return balancedMaps;
    }

    private static Task<TileMap> ChangePickUpsType(TileMap map)
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
                    var tile = map.GetTileWithIndex((int)value.x, (int)value.y);

                    var randomGuess = MapSuggestionMng.RNG.Next(0, 3);
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

        return MapSuggestionMng.pickUpsTask;
    }
}
