using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NumSharp;
using System;
using System.Threading.Tasks;
using static TFModel;
using TileMapLogic;
using static AuthoringTool;

public class MLSuggestionsMng : MonoBehaviour
{
    private enum pickups { none, health, armor, damage};

    public static Task<CharacterParams[]> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        return Task.Run(() =>
        {

            var classes = new CharacterParams[2];
            var thresshold = 0.5f;
            float score = 0;
            var scores = new List<float>();
            foreach (var matchup in classMatchups)
            {
                var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                score = TFModel.PredictKillRatio(input_map, input_weapons);
                scores.Add(score);
            }

            int resultIdx = GetClosestIdxToThresshold(thresshold, scores);

            classes[0] = classMatchups[resultIdx][0]; // red team.
            classes[1] = classMatchups[resultIdx][1]; // blue team.

            return classes;
        });
    }

    private static int GetClosestIdxToThresshold(float thresshold, List<float> scores)
    {
        var closest = scores.Aggregate((x, y) => Mathf.Abs(x - thresshold) < Mathf.Abs(y - thresshold) ? x : y);
        var resultIdx = scores.IndexOf(closest);
        return resultIdx;
    }

    // TODO : More than one tile are spawned inside the region.
    public static Tile[,] SpawnBalancedPickUps(TileMap tilemapMain)
    {
        TileMap map;
        Dictionary<TileMap, float> mapsDict = new Dictionary<TileMap, float>();
        float thresshold = 0.5f;
        float score = 0.0f;
        int diceRoll;
        System.Random RNG = new System.Random();

        // randomly select a region to spawn a pickups

        for (int m = 0; m < 20; m++)
        {
            // this will erase all previous decorations on the main map.
            tileMapMain.RemoveDecorations();
            map = new TileMap(tileMapMain.GetTileMap());
            // iterate regions.
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    diceRoll = RNG.Next(1, 5);
                    if (diceRoll == (int)pickups.health)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.healthPack;
                        map.SetTileMapTile(fillTile);
                    }
                    else if (diceRoll == (int)pickups.armor)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.armorVest;
                        map.SetTileMapTile(fillTile);
                    }
                    else if (diceRoll == (int)pickups.damage)
                    {
                        Tile fillTile = map.GetRandomRegionCell(i, j);
                        fillTile.decID = TileEnums.Decorations.damageBoost;
                        map.SetTileMapTile(fillTile);
                    }
                }
            }
            score = PredictKillRatio(GetInputMap(map), GetInputWeapons(blueClass, redClass));
            mapsDict.Add(map, score);
        }
        var valueslist = mapsDict.Values.ToList();
        var bestMatch = valueslist[GetClosestIdxToThresshold(thresshold,valueslist)];
        var balancedMap = mapsDict.FirstOrDefault(x => x.Value == bestMatch);
        return balancedMap.Key.GetTileMap();
    }
}
