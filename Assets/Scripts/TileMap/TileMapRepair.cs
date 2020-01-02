using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;

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
            && !PathManager.Instance.IsPath(red_base, red_goal))
        {
            return false;
        }

        // 2: Every player must reach all power ups.
        /* Find all power ups to different lists (Tilemap) 
         * loop for every list is path to power up exists from bases
         * 
        */

        // 3: First floor platform should always be connected to a stair
        /* 
         * Locate the platform tiles.
         * Check with flood fill if neighbours have adjucent stairs.
        */

        // 4: Make sure player can get out of first floor rounded areas (a stair exists)
        /*
         * locate rounded first floor areas
         * flood fill to find if inside neighbors have at least one stair
         */

        // 5: A stair should always lead to first floor tiles
        return true;
    }
}
