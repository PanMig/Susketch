using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;
using Random = System.Random;

public class ModifyType : IPowerupPlacement
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

            for (int m = 0; m < GENERATIONS; m++)
            {
                var map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                var map1 = ChangePickUpsType(tempMap);
                var score = PredictKillRatioSynchronous(GetInputMap(map), GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass));
                mapsDict.Add(map, score);
            }

            var balancedMaps = (from pair in mapsDict
                orderby Math.Abs(pair.Value - THRESHOLD)
                select pair).ToList();
            return balancedMaps;
        });
        return task;
    }

    private static TileMap ChangePickUpsType(TileMap map)
    {
        Random RNG = new Random();
        var pickups = map.GetDecorations();

        foreach (var pickup in pickups)
        {
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
    }
}
