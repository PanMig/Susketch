using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;
using static TFModel;

public class RandomMutation : IPowerupPlacement
{
    private const int GENERATIONS = 12;
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

        var weaponsInput = GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass);

        var task = Task.Run(() =>
        {
            // dictionary to keep the generated maps and scores
            Dictionary<Tile[,], float> mapsDict = new Dictionary<Tile[,], float>(new TileMapComparer());

            //Get a 2D array with all valid for placement ground and first level tiles.
            var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);

            Tile[,] map;
            float score = 0;

            for (int m = 0; m < GENERATIONS; m++)
            {
                var diceRoll = MapSuggestionMng.RNG.Next(0, 4);
                switch (diceRoll)
                {
                    case 0:
                        map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                        map = ModifyType.ModifyPickupType(map, tempMap.GetDecorations());
                        score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                        break;
                    case 1:
                        map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                        map = ChangePosition.ChangePickUpsPosInRegion(map, placementLocations, tempMap.GetDecorations());
                        score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                        break;
                    case 2:
                        map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
                        map = RegionSwap.ChangePickUpsRegion(map, placementLocations, tempMap.GetDecorations());
                        score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                        break;
                    case 3:
                        map = RemoveOrPlace.RemoveOrPlacePickUp(tempMap, placementLocations);
                        score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                        break;
                    default:
                        Debug.LogError("Wrong index in random Mutation switch statement. Default was choosen");
                        map = RemoveOrPlace.RemoveOrPlacePickUp(tempMap, placementLocations);
                        score = PredictKillRatioSynchronous(GetInputMap(map), weaponsInput);
                        break;
                }
                if (!mapsDict.ContainsKey(map))
                {
                    mapsDict.Add(map, score);
                }

                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        tempMap.GetTileWithIndex(i, j).CopyEnvDec(map[i, j]);
                    }
                }
            }

            var balancedMaps = (from pair in mapsDict
                                orderby Math.Abs(pair.Value - THRESHOLD)
                                select pair).ToList();
            return balancedMaps;
        });
        return task;
    }
}
