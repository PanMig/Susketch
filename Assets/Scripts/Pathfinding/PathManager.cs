using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMapLogic;
using static AuthoringTool;

public class PlayerPathProperties
{
    public int team; // red = 0, blue = 1
    public bool pathActive = false;
    public List<Tile> highlightedTiles = new List<Tile>();
    //public PathManager.Targets target; 

    public PlayerPathProperties(int team)
    {
        this.team = team;
    }

    public void SetpathActive(bool value)
    {
        pathActive = value;
    }
}


public class PathManager : MonoBehaviour
{
    public static PathManager Instance;

    private PlayerPathProperties pathBlueProps = new PlayerPathProperties(1);
    private PlayerPathProperties pathRedProps = new PlayerPathProperties(0);

    public enum Targets {health, armor, damage, stair, enemyBase};
    public int pathTargetBlue;
    public int pathTargetRed;

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
        //EventManagerUI.onTileMapEdit += UnHighlightPaths;
        EventManagerUI.onTileMapEdit += PathHighlightListener;
    }

    public void SetPathTargetBlue(int value)
    {
        pathTargetBlue = value;
    }

    public void SetPathTargetRed(int value)
    {
        pathTargetRed = value;
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
        if (!pathBlueProps.pathActive)
        {
            UnhighlightPath(pathBlueProps);
        }
        else if(pathBlueProps.pathActive)
        {
            PathHighlightListener();
        }
        if (!pathRedProps.pathActive)
        {
            UnhighlightPath(pathRedProps);
        }
        else if(pathRedProps.pathActive)
        {
            PathHighlightListener();
        }
    }

    public void PathHighlightListener()
    {
        if (pathBlueProps.pathActive)
        {
            switch (pathTargetBlue)
            {
                case (int)Targets.enemyBase:
                    Debug.Log("base");
                    IPathFinding enemyPath = new EnemyBasePath();
                    enemyPath.FindShortestPath(new Vector2(19, 0), new Vector2(0, 19), pathBlueProps);
                    break;
                case (int)Targets.health:
                    Debug.Log("health");
                    IPathFinding healthPath = new HealthPath();
                    // zero parameter stands for empty, goal is handled by the class implementation.
                    healthPath.FindShortestPath(new Vector2(19, 0), new Vector2(0, 19), pathBlueProps);
                    break;
                case (int)Targets.armor:
                    Debug.Log("armor");
                    IPathFinding armorPath = new ArmorPath();
                    // zero parameter stands for empty, goal is handled by the class implementation.
                    armorPath.FindShortestPath(new Vector2(19, 0), new Vector2(0, 19), pathBlueProps);
                    break;
                default:
                    break;
            }
        }
        if (pathRedProps.pathActive)
        {
            switch (pathTargetRed)
            {
                case (int)Targets.enemyBase:
                    IPathFinding enemyPath = new EnemyBasePath();
                    enemyPath.FindShortestPath(new Vector2(0, 19), new Vector2(19, 0), pathRedProps);
                    break;
                case (int)Targets.health:
                    IPathFinding healthPath = new HealthPath();
                    // zero parameter stands for empty, goal is handled by the class implementation.
                    healthPath.FindShortestPath(new Vector2(0,19), new Vector2(0, 19), pathRedProps);
                    break;
                case (int)Targets.armor:
                    IPathFinding armorPath = new ArmorPath();
                    // zero parameter stands for empty, goal is handled by the class implementation.
                    armorPath.FindShortestPath(new Vector2(0, 19), new Vector2(0, 19), pathRedProps);
                    break;
                default:
                    break;
            }
        }
    }

    public bool IsPath(Vector2 start, Vector2 goal)
    {
        if (PathUtils.DFS_Iterative(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
             tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain))
        {
            return true;
        }
        return false;
    }

    public void HighlightShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps)
    {
        if (playerProps.highlightedTiles.Count > 0)
        {
            UnhighlightPath(playerProps);
        }
        playerProps.highlightedTiles = PathUtils.BFSGetShortestPath(tileMapMain.GetTileWithIndex((int)start.x, (int)start.y),
            tileMapMain.GetTileWithIndex((int)goal.x, (int)goal.y), tileMapMain);
        HighlightPath(playerProps);
    }

    public void HighlightPath(PlayerPathProperties playerProps)
    {
        var tiles = playerProps.highlightedTiles;
        foreach (var tile in tiles)
        {
            tile.Highlight(playerProps.team);
        }
    }

    public void UnhighlightPath(PlayerPathProperties playerProps)
    {
        var tiles = playerProps.highlightedTiles;
        foreach (var tile in tiles)
        {
            tile.Unhighlight();
        }
    }
}
