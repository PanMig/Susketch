using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using static AuthoringTool;
using static TileEnums;

public class TileMapRepair
{
    private static readonly Vector2 blue_base = new Vector2(19, 0); 
    private static readonly Vector2 blue_goal = new Vector2(0, 19);

    private static readonly Vector2 red_base = new Vector2(0, 19);
    private static readonly Vector2 red_goal = new Vector2(19, 0);

    public static string errorMsg;

    public delegate void OnMapPlayable();
    public static OnMapPlayable onPlayableMap;

    public delegate void OnNotPlayableMap();
    public static OnNotPlayableMap onUnPlayableMap;

    private static void SetErrorMsg(string msg)
    {
        errorMsg = msg;
    }

    private static bool IsTraversable(TileMap map)
    {
        if (!PathManager.Instance.IsPath(blue_base, blue_goal, map)
            || !PathManager.Instance.IsPath(red_base, red_goal, map))
        {
            return false;
        }
        return true;
    }

    public static bool HasAccesiblePowerUps(TileMap map)
    {
        var decorDict = map.GetDecorations();
        List<string> decorKeys = new List<string>
        {
            TileEnums.Decorations.healthPack.ToString(),
            TileEnums.Decorations.armorVest.ToString(),
            TileEnums.Decorations.damageBoost.ToString()
        };

        for (int i = 0; i < decorDict.Count; i++)
        {
            for (int j = 0; j < decorDict[decorKeys[i]].Count; j++)
            {
                if (!PathManager.Instance.IsPath(blue_base, decorDict[decorKeys[i]][j], map)
                    &&
                    !PathManager.Instance.IsPath(red_base, decorDict[decorKeys[i]][j], map))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool ArePlatformsConnectedToStairs(TileMap map)
    {
        var platforms = map.GetFirstFloorPlatforms();
        for (int i = 0; i < platforms.Count; i++)
        {
            int stairCount = 0;
            for (int j = 0; j < platforms[i].Count; j++)
            {
                var neighbours = PathUtils.GetNeighboursCross(platforms[i][j], map);

                for (int n = 0; n < neighbours.Count; n++)
                {
                    if (neighbours[n].decID == TileEnums.Decorations.stairs) { stairCount++; }
                }
            }

            var midPlatTile = platforms[i][platforms[i].Count / 2];
            if (stairCount == 0 || !PathManager.Instance.IsPath(blue_base, new Vector2(midPlatTile.X, midPlatTile.Y), map))
            {
                return false;
            }
        }
        return true;
    }

    private static bool HasExitFromClosedPlatform(TileMap map)
    {
        var boundaries = map.GetBoundaries();
        var tempMap = map.GetTileMapToInt();
        foreach (var bound in boundaries)
        {
            if (bound.envTileID == EnviromentTiles.ground)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 0, -1, tempMap);
            }
            else if (bound.envTileID == EnviromentTiles.level_1)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 1, -2, tempMap);
            }
        }

        int holesCount = 0;
        int holeStairsCount = 0;
        HashSet<Vector2> tilesVisited = new HashSet<Vector2>();
        for (int i = 0; i < TileMap.rows; i++)
        {
            for (int j = 0; j < TileMap.columns; j++)
            {
                Vector2 v = new Vector2(i, j);
                if (tempMap[i, j] == 0 && !tilesVisited.Contains(v))
                {
                    PathUtils.FloodFill(i, j, 0, 3, tempMap);
                    tilesVisited.Add(v);
                    holesCount++;
                }
            }
        }

        for (int i = 0; i < TileMap.rows; i++)
        {
            for (int j = 0; j < TileMap.columns; j++)
            {
                if (tempMap[i, j] == 3)
                {
                    if (map.GetTileWithIndex(i, j).decID == Decorations.stairs)
                    {
                        holeStairsCount++;
                    }
                }
            }
        }

        if (holesCount > holeStairsCount)
        {
            return false;
        }

        return true;
    }

    private static bool AreStairsConnectedToFirstFloor(TileMap map)
    {
        var stairs = map.GetDecoration(TileEnums.Decorations.stairs);

        for (int i = 0; i < stairs.Count; i++)
        {
            var neighbours = PathUtils.GetNeighboursCross(stairs[i], map);
            int lev1Count = 0;
            if(stairs[i].envTileID == TileEnums.EnviromentTiles.level_1 || 
                stairs[i].envTileID == TileEnums.EnviromentTiles.level_2)
            {
                return false;
            }
            for (int j = 0; j < neighbours.Count; j++)
            {
                if (neighbours[j].envTileID == TileEnums.EnviromentTiles.level_2 && (j == 0 || j == 1))
                {
                    return false;
                }
                else if (neighbours[j].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    lev1Count++;
                }
            }
            if (lev1Count > 1)
            {
                return false;
            }
            else if (lev1Count < 1)
            {
                return false;
            }
        }
        return true;
    }

    public static bool CheckTileMap(TileMap map)
    {
        // 1: Bases should be traversable.
        if (!IsTraversable(map))
        {
            SetErrorMsg("Bases are not traversable.");
            return false;
        }

        // 2: Every player must reach all power ups.
        if (!HasAccesiblePowerUps(map))
        {
            SetErrorMsg("Not all power ups are available to the players.");
            return false;
        }

        // 3: First floor platform should always be connected to a stair
        if (!ArePlatformsConnectedToStairs(map))
        {
            SetErrorMsg("Platform is not connected to a stair.");
            return false;
        }

        // 4: A stair should always lead to first floor tiles
        if (!AreStairsConnectedToFirstFloor(map))
        {
            SetErrorMsg("Stair is wrongly connected. It either leads to a wall, is cornered by two or more level1 tiles, " +
                        "or does not lead to first level floor.");
            return false;
        }

        // 5: Make sure player can get out of first floor closed areas (a stair exists inside hole)
        if (!HasExitFromClosedPlatform(map))
        {
            SetErrorMsg("There is no stair in ground region surrounded by first floor tiles.");
            return false;
        }

        

        return true;
    }
}
