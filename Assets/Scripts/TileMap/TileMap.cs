using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Linq;

namespace TileMapLogic
{
    public partial class TileMap
    {
        public static readonly int rows = 20;
        public static readonly int columns = 20;
        private static readonly int CELL_PER_REGION = 5;
        private Tile[,] tileMap;
        private Region[,] regions = new Region[4, 4];


        public TileMap()
        {

        }

        public TileMap(Tile[,] map)
        {
            InitTileMap(null);
            SetTileMap(map);
        }

        public void InitTileMap(Transform gridTransformParent)
        {
            tileMap = new Tile[rows, columns];
            TileThemes tileTheme;
            Decoration dec;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    //ground tile and empty decoration.
                    tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0, 1)];
                    dec = Brush.Instance.decorations[0];
                    tileMap[row, col] = new Tile(tileTheme.prefab, gridTransformParent,
                        tileTheme.envTileID, dec.decorationID, row, col);
                }
            }
        }


        public void InitRegions()
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

        private Tile[,] FillRegion(int stepX, int stepY)
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

        public void PaintRegion(int row, int column, int brushIndex)
        {
            for (int i = 0; i < CELL_PER_REGION; i++)
            {
                for (int j = 0; j < CELL_PER_REGION; j++)
                {
                    regions[row, column].tileSet[i, j].PaintTile(Brush.Instance.brushThemes[brushIndex], this);
                }
            }
        }

        public void PaintRegion(int row, int column, Color color)
        {
            for (int i = 0; i < CELL_PER_REGION; i++)
            {
                for (int j = 0; j < CELL_PER_REGION; j++)
                {
                    regions[row, column].tileSet[i, j].PaintTile(color);
                }
            }
        }

        public void RenderTileMap()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Tile tile = tileMap[i, j];
                    tile.PaintTile(Brush.Instance.brushThemes[(int)tile.envTileID], this);
                    if(tile.decID != TileEnums.Decorations.empty)
                    {
                        tile.PaintDecoration(Brush.Instance.decorations[(int)tile.decID], this);
                    }
                    else
                    {
                        tile.PaintDecoration(Brush.Instance.decorations[0], this);
                    }
                }
            }
        }

        public void FillTileMap()
        {
            TileThemes tileTheme;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    tileTheme = Brush.Instance.brushThemes[UnityEngine.Random.Range(0, 3)];
                    tileMap[row, col].PaintTile(tileTheme, this);
                }
            }
        }

        public void SetDefaultMap(int envIndex, int decorIndex)
        {
            TileThemes tileTheme;
            Decoration decor;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    tileTheme = Brush.Instance.brushThemes[envIndex];
                    decor = Brush.Instance.decorations[decorIndex];
                    tileMap[row, col].PaintTile(tileTheme, this);
                    tileMap[row, col].PaintDecoration(decor, this);
                }
            }
        }

        public Tile GetTileWithIndex(int row, int col)
        {
            return tileMap[row, col];
        }

        public void SetTileMapTile(Tile tile)
        {
            tileMap[tile.X, tile.Y] = tile;
        }

        public string[,] GetTileMapToString()
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

        public int[,] GetTileMapToInt()
        {
            int[,] intMap = new int[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.ground)
                    {
                        intMap[row, col] = 0;
                    }
                    else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_1)
                    {
                        intMap[row, col] = 1;
                    }
                    else if (tileMap[row, col].envTileID == TileEnums.EnviromentTiles.level_2)
                    {
                        intMap[row, col] = 2;
                    }
                }
            }
            return intMap;
        }

        //TODO load tilemap from csv file.
        public void LoadtileMapFromFile(string fileName)
        {

        }

        public Tile GetRandomRegionCell(int regionNumX, int regionNumY)
        {
            System.Random RNG = new System.Random();
            int xRange = regionNumX * 5;
            int yRange = regionNumY * 5;

            //int row = RNG.Next(xRange - 5, xRange - 1);
            //int column = RNG.Next(yRange - 5, yRange - 1);
            int row = UnityEngine.Random.Range(xRange - 5, xRange - 1);
            int column = UnityEngine.Random.Range(yRange - 5, yRange - 1);

            return GetTileWithIndex(row, column);
        }

        public Tile[,] GetTileMap()
        {
            return tileMap;
        }

        public void SetTileMap(Tile[,] map)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tileMap[i, j].SetTile(map[i, j]);
                }
            }
        }

        public void RemoveDecorations()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tileMap[i, j].decID != TileEnums.Decorations.stairs)
                    {
                        tileMap[i, j].RemoveDecoration(this);
                    }
                }
            }
        }

        public Dictionary<string, List<Vector2>> GetDecorations()
        {
            Dictionary<string, List<Vector2>> decorDict = new Dictionary<string, List<Vector2>>();
            var healthPacks = new List<Vector2>();
            var armorPacks = new List<Vector2>();
            var damagePacks = new List<Vector2>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var currTile = tileMap[i, j];
                    if (currTile.decID == TileEnums.Decorations.healthPack)
                    {
                        healthPacks.Add(new Vector2(currTile.X, currTile.Y));
                    }
                    else if (currTile.decID == TileEnums.Decorations.armorVest)
                    {
                        armorPacks.Add(new Vector2(currTile.X, currTile.Y));
                    }
                    else if (currTile.decID == TileEnums.Decorations.damageBoost)
                    {
                        damagePacks.Add(new Vector2(currTile.X, currTile.Y));
                    }
                }
            }
            decorDict.Add(TileEnums.Decorations.healthPack.ToString(), healthPacks);
            decorDict.Add(TileEnums.Decorations.armorVest.ToString(), armorPacks);
            decorDict.Add(TileEnums.Decorations.damageBoost.ToString(), damagePacks);
            return decorDict;
        }

        public List<Tile> GetDecoration(TileEnums.Decorations decID)
        {
            List<Tile> decorTiles = new List<Tile>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Tile tile = GetTileWithIndex(i, j);
                    if (tile.decID == decID)
                    {
                        decorTiles.Add(tile);
                    }
                }
            }
            return decorTiles;
        }

        public List<List<Tile>> GetFirstFloorPlatforms()
        {
            var platformsList = new List<List<Tile>>();
            var tileList = new List<Tile>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var tile = GetTileWithIndex(i, j);

                    if (!tileList.Contains(tile) && tile.envTileID == TileEnums.EnviromentTiles.level_1)
                    {
                        var list = new List<Tile>();
                        list = PathUtils.RecursiveFloodFill(i, j, Brush.Instance.brushThemes[1], list);
                        tileList.AddRange(list);
                        platformsList.Add(list);
                    }
                }
            }
            return platformsList;
        }

        public List<Tile> GetEnviromentTiles(TileEnums.EnviromentTiles tileID)
        {
            var tileList = new List<Tile>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var tile = GetTileWithIndex(i, j);

                    if (tile.envTileID == tileID)
                    {
                        tileList.Add(tile);
                    }
                }
            }
            return tileList;
        }

        public List<Tile> GetBoundaries()
        {
            var list = new List<Tile>();
            // rows
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    list.Add(GetTileWithIndex(i, j));
                }
            }

            // columns
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    list.Add(GetTileWithIndex(i, j));
                }
            }
            return list;
        }

        public void ReadCSVToTileMap(string fileName)
        {
            //Tile[,] map = new Tile[20, 20];
            TextAsset datafile = Resources.Load<TextAsset>(fileName);
            using (var sr = new StreamReader(new MemoryStream(datafile.bytes)))
            {
                for (var i = 0; i < rows; i++)
                {
                    string line;
                    if ((line = sr.ReadLine()) != null)
                    {
                        var splitted = line.Split(',','|');
                        for (var j = 0; j < columns; j++)
                        {
                            //map[i, j] = splitted[j];
                            switch (splitted[j])
                            {
                                case "0":
                                    Tile tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.ground;
                                    tile.decID = TileEnums.Decorations.empty;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "1":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.level_1;
                                    tile.decID = TileEnums.Decorations.empty;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "2":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.level_2;
                                    tile.decID = TileEnums.Decorations.empty;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "0H":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.ground;
                                    tile.decID = TileEnums.Decorations.healthPack;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "0A":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.ground;
                                    tile.decID = TileEnums.Decorations.armorVest;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "0D":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.ground;
                                    tile.decID = TileEnums.Decorations.damageBoost;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "0S":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.ground;
                                    tile.decID = TileEnums.Decorations.stairs;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "1H":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.level_1;
                                    tile.decID = TileEnums.Decorations.healthPack;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "1A":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.level_1;
                                    tile.decID = TileEnums.Decorations.armorVest;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "1D":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.envTileID = TileEnums.EnviromentTiles.level_1;
                                    tile.decID = TileEnums.Decorations.damageBoost;
                                    this.SetTileMapTile(tile);
                                    break;
                                case "1S":
                                    Debug.LogError("stair on top of first level tile");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

    }
}
