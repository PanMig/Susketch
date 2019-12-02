using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileMapLogic.TileMap;
using static AuthoringTool;

public class EnemyBasePath : IPathFinding
{
    public void FindShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps)
    {
        if (playerProps.highlightedTiles.Count > 0)
        {
            PathManager.Instance.UnhighlightPathBetweenPlayerBases(playerProps);
        }

        playerProps.highlightedTiles = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);

        PathManager.Instance.HighlightPathBetweenPlayerBases(playerProps);
    }
}
