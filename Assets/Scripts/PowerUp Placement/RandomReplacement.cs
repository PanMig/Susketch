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

    private const int GENERATIONS = 12;
    private const float THRESHOLD = 0.5f;

    public  Task<List<KeyValuePair<Tile[,], float>>> ChangePowerUps(TileMap tilemapMain)
    {
        MapSuggestionMng.tempView = new GameObject("TempView");
        // create a deep copy of the main tileMap.
        var tempMap = new TileMap();
        tempMap.Init();
        tempMap.InitRegions();
        tempMap.PaintTiles(MapSuggestionMng.tempView.transform, 1.0f);
        tempMap.SetTileMap(tilemapMain.GetTileMap());
        //remove decorations for every new generation
        tempMap.RemoveDecorations();

        var task = Task.Run(() =>
        {
            // dictionary to keep the generated maps and scores
            Dictionary<Tile[,], float> mapsDict = new Dictionary<Tile[,], float>(new TileMapComparer());

            var weapons = GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass);

            //Get a 2D array with all valid for placement ground and first level tiles.
            var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tempMap);

            for (int m = 0; m < GENERATIONS; m++)
            {
                var map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                map = SetPickUpsLocations(map, placementLocations);
                var score = PredictKillRatioSynchronous(GetInputMap(map), weapons);
                mapsDict.Add(map, score);
            }
            
            var balancedMaps = (from pair in mapsDict
                                orderby Math.Abs(pair.Value - THRESHOLD)
                                select pair).ToList();
            return balancedMaps;
        });
        return task;
    }


    public Tile[,] SetPickUpsLocations(Tile[,] map, List<Tile>[,] validLocations)
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
                Tile fillTile = validLocations[i, j][randomIdx];

                if (powerUpRoll == (int)MapSuggestionMng.pickups.health)
                {
                    map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.healthPack;
                }
                else if (powerUpRoll == (int)MapSuggestionMng.pickups.armor)
                {
                    map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.armorVest;
                }
                else if (powerUpRoll == (int)MapSuggestionMng.pickups.damage)
                {
                    map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.damageBoost;
                }
                else
                {
                    map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.empty;
                }
            }
        }
        return map;
    }

    //private Task<Tile[,]> SetPickUpsLocations(Tile[,] map, List<Tile>[,] validLocations)
    //    {
    //        var task = Task.Run(() =>
    //        {
    //            int powerUpRoll;
    //            //iterate regions.
    //            for (int i = 0; i < 4; i++)
    //            {
    //                for (int j = 0; j < 4; j++)
    //                {
    //                    if (validLocations[i, j].Count == 0)
    //                    {
    //                        continue;
    //                    }

    //                    powerUpRoll = MapSuggestionMng.RNG.Next(1, 5);
    //                    var randomIdx = MapSuggestionMng.RNG.Next(0, validLocations[i, j].Count);
    //                    Tile fillTile = validLocations[i, j][randomIdx];

    //                    if (powerUpRoll == (int)MapSuggestionMng.pickups.health)
    //                    {
    //                        map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.healthPack;
    //                    }
    //                    else if (powerUpRoll == (int)MapSuggestionMng.pickups.armor)
    //                    {
    //                        map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.armorVest;
    //                    }
    //                    else if (powerUpRoll == (int)MapSuggestionMng.pickups.damage)
    //                    {
    //                        map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.damageBoost;
    //                    }
    //                    else
    //                    {
    //                        map[fillTile.X, fillTile.Y].decID = TileEnums.Decorations.empty;
    //                    }
    //                }
    //            }
    //            return map;
    //        });
    //        return task;
    //    }
}
