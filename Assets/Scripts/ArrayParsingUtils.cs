using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Numpy;

public static class ArrayParsingUtils
{
    private static List<int[]> directions = new List<int[]>();

    public static void PaintNeighborhood(float[,] mapToPaint, int[] coords, float[,] secondFloorMap)
    {
        mapToPaint[coords[0], coords[1]] = 1;
        foreach(var neighbor in directions){
            var n = new int[2];
            n[0] = coords[0] + neighbor[0];
            n[1] = coords[1] + neighbor[1];
            if(CoordExists(n, mapToPaint) && !IsOccupied(n, secondFloorMap))
            {
                mapToPaint[n[0], n[1]] = 0;
            }
        }
    }

    public static bool CoordExists(int[] coord, float[,] map)
    {
        if (coord[0] > 0 && coord[0] < 20 && coord[1] > 0 && coord[1] < 20)
        {
            return true;
        }
        return false;
    }

    public static bool IsOccupied(int[] coord , float[,] map)
    {
        if(map[coord[0], coord[1]] == 1) { return true; }
        return false;
    }

    public static void ParseToChannelArray(string[,] array)
    {
        float[,] ground_map = new float[20, 20];
        float[,] first_floor_map = new float[20, 20];
        float[,] second_floor_map = new float[20, 20];
        float[,] hp_map = new float[20, 20];
        float[,] armor_map = new float[20, 20];
        float[,] dd_map = new float[20, 20];
        float[,] stairs_map = new float[20, 20];

        //create channels for enviromental tiles.
        for (int row = 0; row < 20; row++)
        {
            for (int col = 0; col < 20; col++)
            {
                if (array[row, col].Contains("0")) { ground_map[row, col] = 1; }
                if (array[row, col].Contains("1")) { first_floor_map[row, col] = 1; }
                if (array[row, col].Contains("2")) { second_floor_map[row, col] = 1; }
            }
        }

        //create channels for pick ups.
        for (int row = 0; row < 20; row++)
        {
            for (int col = 0; col < 20; col++)
            {
                if (array[row, col].Contains("A")) { PaintNeighborhood(armor_map, new int[2] { row, col }, second_floor_map); }
                if (array[row, col].Contains("D")) { PaintNeighborhood(dd_map, new int[2] { row, col }, second_floor_map); }
                if (array[row, col].Contains("H")) { PaintNeighborhood(hp_map, new int[2] { row, col }, second_floor_map); }
                if (array[row, col].Contains("S")) { stairs_map[row, col] = 1; }
            }
        }

        // stack arrays together


    }
}
