using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;

public class RemoveOrPlace : IPowerupPlacement
{
    private const int GENERATIONS = 1;
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

        var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);
        var task = Task.Run(() =>
        {
            // dictionary to keep the generated maps and scores
            Dictionary<Tile[,], float> mapsDict = new Dictionary<Tile[,], float>(new TileMapComparer());

            for (int m = 0; m < GENERATIONS; m++)
            {
                var map = RemoveOrPlacePickUp(tempMap, placementLocations);
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

    public Tile[,] RemoveOrPlacePickUp(TileMap map, List<Tile>[,] validLocations)
    {
        var region = map.GetRandomRegion(-1, -1);
        if (map.Regions[region.Item1, region.Item2].GetPickUpsNumber() == 0)
        {
            var randomIdx = MapSuggestionMng.RNG.Next(0, validLocations[region.Item1, region.Item2].Count);
            var randomTile = validLocations[region.Item1, region.Item2][randomIdx];
            var diceRoll = MapSuggestionMng.RNG.Next(0, 3);
            switch (diceRoll)
            {
                case 0:
                    map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.healthPack;
                    break;
                case 1:
                    map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.armorVest;
                    break;
                case 2:
                    map.GetTileWithIndex(randomTile.X, randomTile.Y).decID = TileEnums.Decorations.damageBoost;
                    break;
            }
        }
        else
        {
            map.Regions[region.Item1, region.Item2].RemovePickups();
        }

        return map.GetTileMap();
    }
}
