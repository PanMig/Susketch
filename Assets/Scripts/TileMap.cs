using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace TileMapLogic
{
    public class TileMap
    {
        public static readonly int rows = 20;
        public static readonly int columns = 20;
        private static Tile[,] tileMap;
        private static Region[,] regions = new Region[4, 4];
        private static readonly int CELL_PER_REGION = 5;
        private static readonly int REGIONS = 16;

        public static void InitTileMap(Transform GridTransformParent)
        {
            tileMap = new Tile[rows, columns];
            TileThemes tileTheme;
            Decoration dec;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0, 1)];
                    dec = Brush.Instance.decorations[0];
                    tileMap[row, col] = new Tile(tileTheme.prefab, GridTransformParent, tileTheme.envTileID, dec.decorationID, row, col);
                }
            }
        }

        public static void InitRegions()
        {
            int stepX = 0, stepY = 0;
            for (int i = 0; i < 4; i++)
            {
                stepX = i * 5;
                for (int j = 0; j < 4; j++)
                {
                    stepY = j * 5;
                    regions[i, j] = new Region(FillRegion(stepX, stepY));
                }
            }
        }

        private static Tile[,] FillRegion(int stepX, int stepY)
        {
            Tile[,] tileSet = new Tile[5, 5];
            for (int row = 0; row < CELL_PER_REGION; row++)
            {
                for (int col = 0; col < CELL_PER_REGION; col++)
                {
                    tileSet[row, col] = tileMap[row + stepX, col + stepY];
                }
            }
            return tileSet;
        }

        public static void PaintRegion(int row, int column, int brushIndex)
        {
            for (int i = 0; i < CELL_PER_REGION; i++)
            {
                for (int j = 0; j < CELL_PER_REGION; j++)
                {
                    regions[row, column].tileSet[i, j].SetTile(Brush.Instance.brushThemes[brushIndex]);
                }
            }
        }

        public static void PaintRegion(int row, int column, Color color)
        {
            for (int i = 0; i < CELL_PER_REGION; i++)
            {
                for (int j = 0; j < CELL_PER_REGION; j++)
                {
                    regions[row, column].tileSet[i, j].SetTile(color);
                }
            }
        }

        public static void FillTileMap()
        {
            TileThemes tileTheme;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0, 3)];
                    tileMap[row, col].SetTile(tileTheme);
                }
            }
        }

        public static Tile GetTileWithIndex(int row, int col)
        {
            return tileMap[row, col];
        }

        public static void SetTileMapTile(int row, int col, Tile tile)
        {
            tileMap[row, col] = tile;
        }

        public static string[,] GetTileMapToString()
        {
            string[,] stringMap = new string[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.ground)
                    {
                        stringMap[row, col] = "0";
                    }
                    else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_1)
                    {
                        stringMap[row, col] = "1";
                    }
                    else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_2)
                    {
                        stringMap[row, col] = "2";
                    }
                    if (tileMap[row, col].decID == TileEnums.Decorations.healthPack)
                    {
                        stringMap[row, col] += "H";
                    }
                    if (tileMap[row, col].decID == TileEnums.Decorations.damageBoost)
                    {
                        stringMap[row, col] += "D";
                    }
                    if (tileMap[row, col].decID == TileEnums.Decorations.armorVest)
                    {
                        stringMap[row, col] += "A";
                    }
                    if (tileMap[row, col].decID == TileEnums.Decorations.stairs)
                    {
                        stringMap[row, col] += "S";
                    }
                }
            }
            return stringMap;
        }

        //TODO load tilemap from csv file.
        public static void LoadtileMapFromFile(string fileName)
        {

        }

        public static Tile GetRandomRegionCell(int regionNumX, int regionNumY)
        {
            int xRange = regionNumX * 5;
            int yRange = regionNumY * 5;

            int row = UnityEngine.Random.Range(xRange - 5, xRange - 1);
            int column = UnityEngine.Random.Range(yRange - 5, yRange - 1);

            return GetTileWithIndex(row, column);
        }
    }
}
