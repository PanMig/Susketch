using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using NumSharp;
using TileMapLogic;
using UnityEngine;
using static TFModel;

public class Individual
{
    private Tile[,] _map;
    public Tile[,] Map
    {
        get => _map;
        set => _map = SetMap(value);
    }
    public float Fitness { get; set; }

    public Individual(Tile[,] map, float fitness)
    {
        Map = map;
        Fitness = fitness;
    }

    public Tile[,] SetMap(Tile[,] map)
    {
        return TileMap.GetMapDeepCopy(map);
    }
}

public class RandomMutation : IPowerupPlacement
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
        var weaponsInput = GetInputWeapons(CharacterClassMng.Instance.BlueClass, CharacterClassMng.Instance.RedClass);

        // create initial population
        Tile[,] map = TileMap.GetMapDeepCopy(tempMap.GetTileMap());
        var indv = new Individual(map, 0);
        var population = new List<Individual>(){indv, indv};
        
        //Get a 2D array with all valid for placement ground and first level tiles.
        var placementLocations = MapSuggestionMng.GetValidPlacementLocations(tilemapMain);

        var task = Task.Run(() =>
        {
            for (int m = 0; m < GENERATIONS; m++)
            {
                var fit_1 = GetFitnessScore(population[0].Map, weaponsInput);
                var fit_2 = GetFitnessScore(population[1].Map, weaponsInput);

                //Set fitness scores
                population[0].Fitness = fit_1;
                population[1].Fitness = fit_2;

                population = population.OrderByDescending(x => x.Fitness).ToList();

                population.RemoveAt(1);
                population.Add(new Individual(MutateRandomly(map, placementLocations),0));
            }

            var score = PredictKillRatioSynchronous(GetInputMap(population[0].Map), weaponsInput);
            var balancedMaps = new List<KeyValuePair<Tile[,], float>>();
            balancedMaps.Add(new KeyValuePair<Tile[,], float>(population[0].Map, score));
            return balancedMaps;
        });
        return task;
    }

    private static Tile[,] MutateRandomly(Tile[,] map, List<Tile>[,] placementLocations)
    {
        var diceRoll = MapSuggestionMng.RNG.Next(0, 4);
        switch (diceRoll)
        {
            case 0:
                map = ModifyType.ModifyPickupType(map, MapSuggestionMng.GetDecorations(map));
                break;
            case 1:
                map = ChangePosition.ChangePickUpsPosInRegion(map, placementLocations,
                    MapSuggestionMng.GetDecorations(map));
                break;
            case 2:
                map = RegionSwap.ChangePickUpsRegion(map, placementLocations, MapSuggestionMng.GetDecorations(map));
                break;
            case 3:
                map = RemoveOrPlace.RemoveOrPlacePickUp(map, placementLocations);
                break;
            default:
                Debug.LogError("Wrong index in random Mutation switch statement. Default was choosen");
                map = RemoveOrPlace.RemoveOrPlacePickUp(map, placementLocations);
                break;
        }

        return map;
    }

    public float GetFitnessScore(Tile[,] map, NDArray weapons)
    {
        var score = PredictKillRatioSynchronous(GetInputMap(map), weapons);
        return 1 - Mathf.Abs(0.5f - score);
    }
}
