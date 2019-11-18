﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using static AuthoringTool;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }
    private List<Tile> highlightedTiles = new List<Tile>();
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


    private void Update()
    {
        
    }

    public void PathHighlighterHandler()
    {
        HighlightShortestPath(new Vector2(19, 0), new Vector2(0, 19));
    }

    public bool IsMapPlayable(Vector2 start, Vector2 goal)
    {
       PathUtils.BFS(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y), 
           tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);
       return true;
    }

    public void HighlightShortestPath(Vector2 start, Vector2 goal)
    {
        if(highlightedTiles.Count > 0)
        {
            UnhighlightPathBetweenPlayerBases();
        }
        highlightedTiles = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y), 
            tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);
        HighlightPathBetweenPlayerBases();
    }

    private void HighlightPathBetweenPlayerBases()
    {
        foreach (var tile in highlightedTiles)
        {
            tile.Highlight();
        }
    }

    private void UnhighlightPathBetweenPlayerBases()
    {
        foreach (var tile in highlightedTiles)
        {
            tile.Unhighlight();
        }
    }
}
