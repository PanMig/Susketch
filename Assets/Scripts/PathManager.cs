﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using static AuthoringTool;

public class PlayerPathProperties
{
    public bool pathActive = false;
    public List<Tile> highlightedTiles = new List<Tile>();
    public PathManager.Targets target; 

    public PlayerPathProperties()
    {
        
    }

    public void SetpathActive(bool value)
    {
        pathActive = value;
    }
}


public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    private PlayerPathProperties pathBlueProps = new PlayerPathProperties();
    private PlayerPathProperties pathRedProps = new PlayerPathProperties();

    public enum Targets {health, armor, damage, enemyBase, stair };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        EventManagerUI.onTileMapEdit += PathHighlightListener;
        EventManagerUI.onTileMapEdit += UnHighlightPaths;
    }

    public void SetBluePathActivation(bool value)
    {
        pathBlueProps.pathActive = value;
    }

    public void SetRedPathActivation(bool value)
    {
        pathRedProps.pathActive = value;
    }

    public void UnHighlightPaths()
    {
        Debug.Log("Unhighlight paths");
        if (!pathBlueProps.pathActive)
        {
            UnhighlightPathBetweenPlayerBases(pathBlueProps);
        }
        if (!pathRedProps.pathActive)
        {
            UnhighlightPathBetweenPlayerBases(pathRedProps);
        }
    }

    public void PathHighlightListener()
    {
        if (pathBlueProps.pathActive)
        {
            //HighlightShortestPath(new Vector2(19, 0), new Vector2(0, 19), pathBlueProps);
            IPathFinding enemyPath = new EnemyBasePath();
            enemyPath.FindShortestPath(new Vector2(19, 0), new Vector2(0, 19), pathBlueProps);

        }
        if (pathRedProps.pathActive)
        {
            HighlightShortestPath(new Vector2(0, 19), new Vector2(19, 0), pathRedProps);
        }
    }

    public bool IsMapPlayable(Vector2 start, Vector2 goal)
    {
       PathUtils.BFS(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y), 
           tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);
       return true;
    }

    public void HighlightShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps)
    {
        if (playerProps.highlightedTiles.Count > 0)
        {
            UnhighlightPathBetweenPlayerBases(playerProps);
        }
        playerProps.highlightedTiles = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);
        HighlightPathBetweenPlayerBases(playerProps);
    }

    public void HighlightPathBetweenPlayerBases(PlayerPathProperties playerProps)
    {
        var tiles = playerProps.highlightedTiles;
        foreach (var tile in tiles)
        {
            tile.Highlight();
        }
    }

    public void UnhighlightPathBetweenPlayerBases(PlayerPathProperties playerProps)
    {
        var tiles = playerProps.highlightedTiles;
        foreach (var tile in tiles)
        {
            tile.Unhighlight();
        }
    }
}
