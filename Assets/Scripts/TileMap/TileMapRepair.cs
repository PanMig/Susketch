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
    public static event OnMapPlayable onPlayableMap;

    public delegate void OnNotPlayableMap();
    public static event OnNotPlayableMap onUnPlayableMap;

    private static void SetErrorMsg(string msg)
    {
        errorMsg = msg;
    }

    private static bool IsTraversable()
    {
        if (!PathManager.Instance.IsPath(blue_base, blue_goal)
            || !PathManager.Instance.IsPath(red_base, red_goal))
        {
            return false;
        }
        return true;
    }

    private static bool HasAccesiblePowerUps()
    {
        var decorDict = tileMapMain.GetDecorations();
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
                if (!PathManager.Instance.IsPath(blue_base, decorDict[decorKeys[i]][j])
                    &&
                    !PathManager.Instance.IsPath(red_base, decorDict[decorKeys[i]][j]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool ArePlatformsConnectedToStairs()
    {
        var platforms = tileMapMain.GetFirstFloorPlatforms();
        for (int i = 0; i < platforms.Count; i++)
        {
            int stairCount = 0;
            for (int j = 0; j < platforms[i].Count; j++)
            {
                var neighbours = PathUtils.GetNeighboursCross(platforms[i][j], tileMapMain);
                for (int n = 0; n < neighbours.Count; n++)
                {
                    if (neighbours[n].decID == TileEnums.Decorations.stairs) { stairCount++; }
                }
            }
            if (stairCount == 0)
            {
                return false;
            }
        }
        return true;
    }

    private static bool HasExitFromClosedPlatform()
    {
        var boundaries = tileMapMain.GetBoundaries();
        var map = tileMapMain.GetTileMapToInt();
        foreach (var bound in boundaries)
        {
            if (bound.envTileID == EnviromentTiles.ground)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 0, -1, map);
            }
            else if (bound.envTileID == EnviromentTiles.level_1)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 1, -2, map);
            }
        }

        int holesCount = 0;
        int stairCount = 0;
        HashSet<Vector2> tilesVisited = new HashSet<Vector2>();
        for (int i = 0; i < TileMap.rows; i++)
        {
            for (int j = 0; j < TileMap.columns; j++)
            {
                Vector2 v = new Vector2(i, j);
                if (map[i, j] == 0 && !tilesVisited.Contains(v))
                {
                    PathUtils.FloodFill(i, j, 0, 3, map);
                    tilesVisited.Add(v);
                    holesCount++;
                    if (tileMapMain.GetTileWithIndex(i, j).decID == Decorations.stairs)
                    {
                        stairCount++;
                    }
                }
            }
        }

        return true;
    }

    private static bool AreStairsConnectedToFirstFloor()
    {
        var stairs = tileMapMain.GetDecoration(TileEnums.Decorations.stairs);

        for (int i = 0; i < stairs.Count; i++)
        {
            var neighbours = PathUtils.GetNeighboursCross(stairs[i], tileMapMain);
            int lev1Count = 0;
            if(stairs[i].envTileID == TileEnums.EnviromentTiles.level_1 || stairs[i].envTileID == TileEnums.EnviromentTiles.level_2)
            {
                return false;
            }
            for (int j = 0; j < neighbours.Count; j++)
            {
                if (neighbours[j].envTileID == TileEnums.EnviromentTiles.level_2)
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

    public static bool CheckTileMap()
    {
        // 1: Bases should be traversable.
        if (!IsTraversable())
        {
            SetErrorMsg("Bases are not traversable");
            onUnPlayableMap?.Invoke();
            return false;
        }

        // 2: Every player must reach all power ups.
        if (!HasAccesiblePowerUps())
        {
            SetErrorMsg("Not all power ups are available to players");
            onUnPlayableMap?.Invoke();
            return false;
        }

        // 3: First floor platform should always be connected to a stair
        if (!ArePlatformsConnectedToStairs())
        {
            SetErrorMsg("Platform not connected to a stair");
            onUnPlayableMap?.Invoke();
            return false;
        }

        // 4: Make sure player can get out of first floor closed areas (a stair exists inside hole)
        if (!HasExitFromClosedPlatform())
        {
            SetErrorMsg("There is not exit from first floor region");
            onUnPlayableMap?.Invoke();
            return false;
        }

        // 5: A stair should always lead to first floor tiles
        if (!AreStairsConnectedToFirstFloor())
        {
            SetErrorMsg("Stair is wrongly connected. It either leads to a wall, is cornered by two or more level1 tiles, " +
                "or does not lead to first level floor");
            onUnPlayableMap?.Invoke();
            return false;
        }

        onPlayableMap?.Invoke();
        return true;
    }
}
