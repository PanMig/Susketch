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
        private static readonly int REGION_ROWS = 4;
        private static readonly int REGION_COLS = 4;
        protected Tile[,] tileMap;
        public Region[,] Regions = new Region[4, 4];


        public TileMap()
        {
            Init();
        }

        public virtual void Init()
        {
            tileMap = new Tile[rows, columns];
            TileThemes tileTheme;
            Decoration dec;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    //ground tile and empty decoration.
                    tileTheme = Brush.Instance.brushThemes[0];
                    dec = Brush.Instance.decorations[0];
                    tileMap[row, col] = new Tile(tileTheme.envTileID, dec.decorationID, row, col);
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
                    Regions[i, j] = new Region(FillRegion(stepX, stepY));
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
                    Regions[row, column].tileSet[i, j].SetTheme(Brush.Instance.brushThemes[brushIndex]);
                }
            }
        }

        public void PaintRegion(int row, int column, Color color)
        {
            for (int i = 0; i < CELL_PER_REGION; i++)
            {
                for (int j = 0; j < CELL_PER_REGION; j++)
                {
                    Regions[row, column].tileSet[i, j].SetColor(color);
                }
            }
        }

        public void PaintRegionBorders(int regionRowIdx, int regionColIdx, int brushIndex)
        {
            var region = Regions[regionRowIdx, regionColIdx];
            var perimtTiles = region.GetPerimetricTiles();
            for (int i = 0; i < perimtTiles.Count; i++)
            {
                perimtTiles[i].SetTheme(Brush.Instance.brushThemes[brushIndex]);
            }
        }

        public virtual void PaintTiles(Transform parent, float decorationScale)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tileMap[i, j].PaintTile(Brush.Instance.brushThemes[0].prefab,
                        Brush.Instance.brushThemes[0], Brush.Instance.decorations[0], parent, decorationScale);
                }
            }
        }

        public virtual void Render()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tileMap[i, j].Render();
                }
            }
        }

        public void SetDefaultMap(int envIndex, int decorIndex, Transform parent)
        {
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var tileTheme = Brush.Instance.brushThemes[envIndex];
                    var decor = Brush.Instance.decorations[decorIndex];
                    tileMap[row, col].SetTheme(tileTheme);
                    tileMap[row, col].SetDecoration(decor);
                }
            }
        }

        public Tile GetTileWithIndex(int row, int col)
        {
            return tileMap[row, col];
        }

        public static Tile[,] GetMapDeepCopy(Tile[,] inputMap)
        {
            var map = new Tile[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    map[i, j] = new Tile(TileEnums.EnviromentTiles.ground, TileEnums.Decorations.empty, i, j);
                    map[i, j].CopyEnvDec(inputMap[i, j]);
                }
            }

            return map;
        }

        public static void RemoveMapDecorations(Tile[,] map)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (map[i, j].decID != TileEnums.Decorations.stairs &&
                        map[i, j].decID != TileEnums.Decorations.empty)
                    {
                        map[i, j].SetDecoration(Brush.Instance.decorations[0]);
                    }
                }
            }
        }

        public static string[,] GetTileMapToString(Tile[,] map)
        {
            string[,] stringMap = new string[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (map[row, col].envTileID == TileEnums.EnviromentTiles.ground)
                    {
                        stringMap[row, col] = "0";
                    }
                    else if (map[row, col].envTileID == TileEnums.EnviromentTiles.level_1)
                    {
                        stringMap[row, col] = "1";
                    }
                    else if (map[row, col].envTileID == TileEnums.EnviromentTiles.level_2)
                    {
                        stringMap[row, col] = "2";
                    }

                    if (map[row, col].decID == TileEnums.Decorations.healthPack)
                    {
                        stringMap[row, col] += "H";
                    }

                    if (map[row, col].decID == TileEnums.Decorations.damageBoost)
                    {
                        stringMap[row, col] += "D";
                    }

                    if (map[row, col].decID == TileEnums.Decorations.armorVest)
                    {
                        stringMap[row, col] += "A";
                    }

                    if (map[row, col].decID == TileEnums.Decorations.stairs)
                    {
                        stringMap[row, col] += "S";
                    }
                }
            }

            return stringMap;
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

        //public Tuple<int,int> GetTileRegion(int tile_X, int tile_Y)
        //{
        //    int step = 5;
        //    int regions = 4;

        //    int row_idx = tile_X / step;
        //    int col_idx = tile_Y / step;
        //    return new Tuple<int, int>(row_idx, col_idx);
        //}

        //public Tile GetRandomRegionCell(int regionNumX, int regionNumY, System.Random RNG)
        //{
        //    int xRange = regionNumX * 5;
        //    int yRange = regionNumY * 5;

        //    int row = RNG.Next(xRange - 5, xRange - 1);
        //    int column = RNG.Next(yRange - 5, yRange - 1);

        //    var tile = GetTileWithIndex(row, column);

        //    return tile;
        //}

        //public Tuple<int, int> GetRandomRegionWithNoPowerUps(int removeX, int removeY, List<Tile>[,] validLocations)
        //{
        //    var rand = new System.Random();
        //    int row, col = 0;

        //    var rangeX = Enumerable.Range(0, 4).Where(i => i != removeY);
        //    var rangeY = Enumerable.Range(0, 4).Where(i => i != removeX);

        //    List<Tuple<int, int>> validRegions = new List<Tuple<int, int>>();

        //    foreach (var x in rangeX)
        //    {
        //        foreach (var y in rangeY)
        //        {
        //            if (Regions[x, y].GetPickUpsNumber() == 0 && validLocations[x, y].Count > 0)
        //            {
        //                validRegions.Add(new Tuple<int, int>(x, y));
        //            }
        //        }
        //    }

        //    if (validRegions.Count > 0)
        //    {
        //        int random_idx = rand.Next(0, validRegions.Count);
        //        return validRegions[random_idx];
        //    }
        //    return new Tuple<int, int>(-1, -1);
        //}

        public virtual Tile[,] GetTileMap()
        {
            return tileMap;
        }

        public virtual void SetTileMap(Tile[,] map)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tileMap[i, j].SetTile(map[i, j]);
                }
            }
        }

        public void CopyTileMap(Tile[,] map)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tileMap[i, j].CopyEnvDec(map[i, j]);
                }
            }
        }

        public void RemoveDecorations()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tileMap[i, j].decID != TileEnums.Decorations.stairs &&
                        tileMap[i, j].decID != TileEnums.Decorations.empty)
                    {
                        tileMap[i, j].SetDecoration(Brush.Instance.decorations[0]);
                    }
                }
            }
        }

        public Dictionary<string, List<Vector2>> GetDecorationsCoordinates()
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

        public Dictionary<string, List<Tile>> GetDecorations()
        {
            Dictionary<string, List<Tile>> decorDict = new Dictionary<string, List<Tile>>();
            var healthPacks = new List<Tile>();
            var armorPacks = new List<Tile>();
            var damagePacks = new List<Tile>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var currTile = tileMap[i, j];
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

        public Dictionary<string, int> GetDecorationsCount()
        {
            Dictionary<string, int> decorDict = new Dictionary<string, int>();
            var healthPacks = 0;
            var armorPacks =  0;
            var damagePacks = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var currTile = tileMap[i, j];
                    if (currTile.decID == TileEnums.Decorations.healthPack)
                    {
                        healthPacks++;
                    }
                    else if (currTile.decID == TileEnums.Decorations.armorVest)
                    {
                        armorPacks++;
                    }
                    else if (currTile.decID == TileEnums.Decorations.damageBoost)
                    {
                        damagePacks++;
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

        public List<List<Tile>> GetFirstFloorPlatformBounds()
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

        public void ReadCSVToTileMap(string fileName, bool fromAssets)
        {
            FileStream file;
            if (fromAssets)
            {
                var path = Application.streamingAssetsPath + "/" + fileName;
                file = new FileStream(path, FileMode.Open, FileAccess.Read);

            }
            else
            {
                var path = Application.dataPath + "/" + fileName;
                file = new FileStream(path, FileMode.Open, FileAccess.Read);
            }

            using (var sr = new StreamReader(file))
            {
                for (var i = 0; i < rows; i++)
                {
                    string line;
                    if ((line = sr.ReadLine()) != null)
                    {
                        var splitted = line.Split(',', '|');
                        for (var j = 0; j < columns; j++)
                        {
                            //map[i, j] = splitted[j];
                            switch (splitted[j])
                            {
                                case "0":
                                    Tile tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[0]);
                                    tile.SetDecoration(Brush.Instance.decorations[0]);
                                    break;
                                case "1":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[1]);
                                    tile.SetDecoration(Brush.Instance.decorations[0]);
                                    this.SetTileMapTile(tile);
                                    break;
                                case "2":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[2]);
                                    tile.SetDecoration(Brush.Instance.decorations[0]);
                                    break;
                                case "0H":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[0]);
                                    tile.SetDecoration(Brush.Instance.decorations[1]);
                                    break;
                                case "0A":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[0]);
                                    tile.SetDecoration(Brush.Instance.decorations[2]);
                                    break;
                                case "0D":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[0]);
                                    tile.SetDecoration(Brush.Instance.decorations[3]);
                                    break;
                                case "0S":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[0]);
                                    tile.SetDecoration(Brush.Instance.decorations[4]);
                                    break;
                                case "1H":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[1]);
                                    tile.SetDecoration(Brush.Instance.decorations[1]);
                                    break;
                                case "1A":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[1]);
                                    tile.SetDecoration(Brush.Instance.decorations[2]);
                                    break;
                                case "1D":
                                    tile = this.GetTileWithIndex(i, j);
                                    tile.SetTheme(Brush.Instance.brushThemes[1]);
                                    tile.SetDecoration(Brush.Instance.decorations[3]);
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

        public void ExportTileMapToCSV(string fileName)
        {
            StreamWriter file = new StreamWriter(Application.dataPath + $"/{fileName}.csv", false);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var env = tileMap[i, j].envTileID;
                    switch (env)
                    {
                        case TileEnums.EnviromentTiles.ground:
                            file.Write("0");
                            break;
                        case TileEnums.EnviromentTiles.level_1:
                            file.Write("1");
                            break;
                        case TileEnums.EnviromentTiles.level_2:
                            file.Write("2");
                            break;
                    }

                    if (tileMap[i, j].decID != TileEnums.Decorations.empty)
                    {
                        var dec = tileMap[i, j].decID;
                        switch (dec)
                        {
                            case TileEnums.Decorations.healthPack:
                                file.Write("H");
                                break;
                            case TileEnums.Decorations.armorVest:
                                file.Write("A");
                                break;
                            case TileEnums.Decorations.damageBoost:
                                file.Write("D");
                                break;
                            case TileEnums.Decorations.stairs:
                                file.Write("S");
                                break;
                        }
                    }

                    file.Write(",");
                }

                //go to next line
                file.Write("\n");
            }

            file.Close();
            Debug.Log("Saved map file to: " + Application.dataPath + $"/{fileName}.csv");
        }

        public string[,] ExportToStringArray()
        {
            string[,] map = new string[20, 20];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var env = tileMap[i, j].envTileID;
                    switch (env)
                    {
                        case TileEnums.EnviromentTiles.ground:
                            map[i, j] = "0";
                            break;
                        case TileEnums.EnviromentTiles.level_1:
                            map[i, j] = "1";
                            break;
                        case TileEnums.EnviromentTiles.level_2:
                            map[i, j] = "2";
                            break;
                    }

                    if (tileMap[i, j].decID != TileEnums.Decorations.empty)
                    {
                        var dec = tileMap[i, j].decID;
                        switch (dec)
                        {
                            case TileEnums.Decorations.healthPack:
                                map[i, j] = $"{map[i, j]}H";
                                break;
                            case TileEnums.Decorations.armorVest:
                                map[i, j] = $"{map[i, j]}A";
                                break;
                            case TileEnums.Decorations.damageBoost:
                                map[i, j] = $"{map[i, j]}D";
                                break;
                            case TileEnums.Decorations.stairs:
                                map[i, j] = $"{map[i, j]}S";
                                break;
                        }
                    }
                }
            }

            return map;
        }
    }
}
