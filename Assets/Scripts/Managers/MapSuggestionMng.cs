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

public class MapSuggestionMng : MonoBehaviour
{
    public enum pickups { none, health, armor, damage };
    public static Task<Tile[,]> pickUpsTask;
    public static bool pickUpsTaskBusy;
    public static bool classBalanceTaskBusy;
    public static GameObject tempView;

    public static System.Random RNG = new System.Random();
    public static float thresshold = 0.5f;
    private static float score = 0.0f;

    #region events

    public delegate void OnCharactersBalanced(bool value);
    public static OnCharactersBalanced onCharactersBalanced;

    public delegate void OnPickUpsGenerated(bool value);
    public static event OnPickUpsGenerated onPickUpsGenerated;

    #endregion

    #region Class Balance

    public static Task<KeyValuePair<CharacterParams[], float>> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray inputMap)
    {
        return Task.Run(async () =>
        {
            var classes = new CharacterParams[2];
            var thresshold = 0.5f;
            float score = 0;
            var scores = new List<float>();
            foreach (var matchup in classMatchups)
            {
                var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                score = await PredictKillRatio(inputMap, input_weapons);
                scores.Add(score);
            }

            int resultIdx = GetClosestIdxToThresshold(thresshold, scores);

            classes[0] = classMatchups[resultIdx][0]; // red team.
            classes[1] = classMatchups[resultIdx][1]; // blue team.
            var classPair = new KeyValuePair<CharacterParams[], float>(classes, scores[resultIdx]);
            return classPair;
        });
    }

    public static async Task<KeyValuePair<CharacterParams[], float>> GetBalancedMatchUpAsynchronous(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        classBalanceTaskBusy = true;
        var characterClasses = await GetBalancedMatchup(classMatchups, input_map).ConfigureAwait(false);
        classBalanceTaskBusy = false;
        return characterClasses;
    }

    private static int GetClosestIdxToThresshold(float thresshold, List<float> scores)
    {
        var closest = scores.Aggregate((x, y) => Mathf.Abs(x - thresshold) < Mathf.Abs(y - thresshold) ? x : y);
        var resultIdx = scores.IndexOf(closest);
        return resultIdx;
    }

    #endregion

    #region Pickups Placement

    public static async Task<List<KeyValuePair<Tile[,], float>>> SpawnPickupsAsynchronous(TileMap tilemapMain, Enums.PowerUpPlacement replaceType)
    {
        pickUpsTaskBusy = true;
        onPickUpsGenerated?.Invoke(false);
        List<KeyValuePair<Tile[,], float>> maps;
        if (replaceType == Enums.PowerUpPlacement.randomMutation)
        {
            var rndMutation = new RandomMutation();
            maps = await rndMutation.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.randomReplacement)
        {
            var rndReplacement = new RandomReplacement();
            maps = await rndReplacement.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.regionSwap)
        {
            var regSwap = new RegionSwap();
            maps = await regSwap.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.modifyType)
        {
            var modifyType = new ModifyType();
            maps = await modifyType.ChangePowerUps(tilemapMain);
        }
        else if (replaceType == Enums.PowerUpPlacement.RemoveOrPlace)
        {
            var removeOrPlace = new RemoveOrPlace();
            maps = await removeOrPlace.ChangePowerUps(tilemapMain);
        }
        else
        {
            var changePos = new ChangePosition();
            maps = await changePos.ChangePowerUps(tilemapMain);
        }
        onPickUpsGenerated?.Invoke(true);
        pickUpsTaskBusy = false;
        return maps;
    }

    public static List<Tile>[,] GetValidPlacementLocations(TileMap map)
    {
        const int regionRows = 4, regionCols = 4;
        List<Tile>[,] validLocations = new List<Tile>[regionRows, regionCols];

        for (int i = 0; i < regionRows; i++)
        {
            for (int j = 0; j < regionCols; j++)
            {
                validLocations[i, j] = new List<Tile>();
                var groundTiles = map.Regions[i, j].GetEnvironmentalTiles(TileEnums.EnviromentTiles.ground);
                var floorTiles = map.Regions[i, j].GetEnvironmentalTiles(TileEnums.EnviromentTiles.level_1);
                if (groundTiles.Count > 0)
                {
                    validLocations[i, j].AddRange(groundTiles);
                }
                if (floorTiles.Count > 0)
                {
                    validLocations[i, j].AddRange(floorTiles);
                }
            }
        }
        return validLocations;
    }

    public static Tuple<int, int> GetRandomRegion(int removeX, int removeY)
    {
        var rand = new System.Random();
        int row, col = 0;
        if (removeY != -1 && removeX != -1)
        {
            var rangeX = Enumerable.Range(0, 4).Where(i => i != removeY);
            var rangeY = Enumerable.Range(0, 4).Where(i => i != removeX);

            int row_index = rand.Next(0, 3);
            row = rangeX.ElementAt(row_index);
            int col_index = rand.Next(0, 3);
            col = rangeY.ElementAt(col_index);
            return new Tuple<int, int>(row, col);
        }
        // -1 case means that no region to be removed is given.
        else
        {
            row = rand.Next(0, 3);
            col = rand.Next(0, 3);
            return new Tuple<int, int>(row, col);
        }
    }

    public static Tuple<int, int> GetRandomRegionWithNoPowerUps(int removeX, int removeY, List<Tile>[,] validLocations, Tile[,] map)
    {
        var rand = new System.Random();
        int row, col = 0;

        var rangeX = Enumerable.Range(0, 4).Where(i => i != removeY);
        var rangeY = Enumerable.Range(0, 4).Where(i => i != removeX);

        List<Tuple<int, int>> validRegions = new List<Tuple<int, int>>();

        foreach (var x in rangeX)
        {
            foreach (var y in rangeY)
            {
                if (GetPickupsNumberInRegion(x,y,map) == 0 && validLocations[x, y].Count > 0)
                {
                    validRegions.Add(new Tuple<int, int>(x, y));
                }
            }
        }

        if (validRegions.Count > 0)
        {
            int random_idx = rand.Next(0, validRegions.Count);
            return validRegions[random_idx];
        }
        return new Tuple<int, int>(-1, -1);
    }

    public static int GetPickupsNumberInRegion(int x, int y, Tile[,] map)
    {
        var count = 0;
        for (int i = x * 5; i < x * 5 + 5; i++)
        {
            for (int j = y * 5; j < y * 5 + 5; j++)
            {
                if (map[i, j].decID != TileEnums.Decorations.empty && map[i, j].decID != TileEnums.Decorations.stairs)
                {
                    count++;
                }
            }
        }

        return count;

    }

    public static Tile[,] RemovePickupsInRegion(int x, int y, Tile[,] map)
    {
        for (int i = x * 5; i < x * 5 + 5; i++)
        {
            for (int j = y * 5; j < y * 5 + 5; j++)
            {
                if (map[i, j].decID != TileEnums.Decorations.stairs)
                {
                    map[i, j].decID = TileEnums.Decorations.empty;
                }
            }
        }
        return map;
    }

    public static Dictionary<string, List<Tile>> GetDecorations(Tile[,] map)
    {
        Dictionary<string, List<Tile>> decorDict = new Dictionary<string, List<Tile>>();
        var healthPacks = new List<Tile>();
        var armorPacks = new List<Tile>();
        var damagePacks = new List<Tile>();

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                var currTile = map[i, j];
                if (currTile.decID == TileEnums.Decorations.healthPack)
                {
                    healthPacks.Add(currTile);
                }
                else if (currTile.decID == TileEnums.Decorations.armorVest)
                {
                    armorPacks.Add(currTile);
                }
                else if (currTile.decID == TileEnums.Decorations.damageBoost)
                {
                    damagePacks.Add(currTile);
                }
            }
        }

        decorDict.Add(TileEnums.Decorations.healthPack.ToString(), healthPacks);
        decorDict.Add(TileEnums.Decorations.armorVest.ToString(), armorPacks);
        decorDict.Add(TileEnums.Decorations.damageBoost.ToString(), damagePacks);
        return decorDict;
    }

    #endregion
}


