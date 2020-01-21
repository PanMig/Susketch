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
            PathManager.Instance.UnhighlightPath(playerProps);
            playerProps.highlightedTiles.Clear();
        }

        playerProps.highlightedTiles = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);

        PathManager.Instance.HighlightPath(playerProps);
        playerProps.movementSteps = playerProps.highlightedTiles.Count;
    }
}

public class HealthPath : IPathFinding
{
    public void FindShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps)
    {
        if (playerProps.highlightedTiles.Count > 0)
        {
            PathManager.Instance.UnhighlightPath(playerProps);
            playerProps.highlightedTiles.Clear();
        }
        int count = 0;
        var healthDict = tileMapMain.GetDecoration(TileEnums.Decorations.healthPack);
        foreach (var healthPack in healthDict)
        {
            var tilePath = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex(healthPack.X, healthPack.Y), tileMapMain);
            playerProps.highlightedTiles.AddRange(tilePath);
            count++;
        }
        PathManager.Instance.HighlightPath(playerProps);
        playerProps.movementSteps = count > 0 ? playerProps.highlightedTiles.Count / count : 0;
    }
}


public class ArmorPath : IPathFinding
{
    public void FindShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps)
    {
        if (playerProps.highlightedTiles.Count > 0)
        {
            PathManager.Instance.UnhighlightPath(playerProps);
            playerProps.highlightedTiles.Clear();
        }
        int count = 0;
        var armorDict = tileMapMain.GetDecoration(TileEnums.Decorations.armorVest);
        foreach (var armorPack in armorDict)
        {
            var tilePath = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex(armorPack.X, armorPack.Y), tileMapMain);
            playerProps.highlightedTiles.AddRange(tilePath);
            count ++;
        }
        PathManager.Instance.HighlightPath(playerProps);
        playerProps.movementSteps = count > 0 ? playerProps.highlightedTiles.Count / count : 0;
    }
}
