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

    public static bool CheckTileMap()
    {
        // 1: Bases should be traversable.
        if(!PathManager.Instance.IsPath(blue_base, blue_goal)
            || !PathManager.Instance.IsPath(red_base, red_goal))
        {
            Debug.Log("Bases are not traversable");
            return false;
        }

        // 2: Every player must reach all power ups.
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
                    Debug.Log("Not all power ups are available to players");
                    return false;
                }
            }
        }

        // 3: First floor platform should always be connected to a stair
        var platforms = tileMapMain.GetFirstFloorPlatforms();
        Debug.Log(platforms.Count);
        for (int i = 0; i < platforms.Count; i++)
        {
            int stairCount = 0;
            for (int j = 0; j < platforms[i].Count; j++)
            {
                var neighbours = PathUtils.GetNeighboursCross(platforms[i][j], tileMapMain);
                for (int n = 0; n < neighbours.Count; n++)
                {
                    if(neighbours[n].decID == TileEnums.Decorations.stairs) { stairCount++; }
                }
            }
            if(stairCount < 1)
            {
                Debug.Log("Platform not connected to a stair");
                return false;
            }
        }

        // 4: Make sure player can get out of first floor rounded areas (a stair exists)
        var boundaries = tileMapMain.GetBoundaries();
        var map = tileMapMain.GetTileMapToInt();
        var visited = new HashSet<Tile>();
        foreach (var bound in boundaries)
        {
            if (bound.envTileID == EnviromentTiles.ground)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 0, -1, map);
                visited.Add(bound);
            }
            else if(bound.envTileID == EnviromentTiles.level_1)
            {
                PathUtils.FloodFill(bound.X, bound.Y, 1, -2, map);
                visited.Add(bound);
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if(map[i,j] == 0)
                {
                    Debug.Log("yey");
                }
            }
        }

        // 5: A stair should always lead to first floor tiles
        var stairs = tileMapMain.GetDecoration(TileEnums.Decorations.stairs);

        for (int i = 0; i < stairs.Count; i++)
        {
            var neighbours = PathUtils.GetNeighboursCross(stairs[i], tileMapMain);
            int lev1Count = 0;
            for (int j = 0; j < neighbours.Count; j++)
            {
                if(neighbours[j].envTileID == TileEnums.EnviromentTiles.level_2)
                {
                    Debug.Log("Stairs leads to level 2");
                    return false;
                }
                else if(neighbours[j].envTileID == TileEnums.EnviromentTiles.level_1)
                {
                    lev1Count++;
                }
            }
            if(lev1Count > 1)
            {
                Debug.Log("Stairs are cornered by two or more level1 tiles");
                return false;
            }
            else if(lev1Count < 1)
            {
                Debug.Log("Stairs do not lead to first level floor");
                return false;
            }
        }
        return true;
    }
}
