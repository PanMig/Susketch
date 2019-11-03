using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileMapLogic;

public static class PathUtils
{
    public static bool DFS_Iterative(Tile start, Tile goal)
    {
        Stack<Tile> frontier = new Stack<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        frontier.Push(start);
        while(frontier.Count > 0)
        {
            Tile v = frontier.Pop();
            if (!visited.Contains(v))
            {
                visited.Add(v);
                if (visited.Contains(goal))
                {
                    return true;
                }
                //v.SetTile(Brush.Instance.brushThemes[3]);
                List<Tile> neighbours = GetNeighboursInFourDirections(v);
                foreach (var neighbour in neighbours)
                {
                    frontier.Push(neighbour);
                }
            }
        }

        return false;
    }

    public static Dictionary<Tile,Tile> BFS(Tile start, Tile goal)
    {
        Queue<Tile> frontier = new Queue<Tile>();
        frontier.Enqueue(start);

        Dictionary<Tile,Tile> came_from = new Dictionary<Tile, Tile>();
        came_from.Add(start, start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            List<Tile> neighbours = GetNeighboursInFourDirections(current);
            foreach (var next in neighbours)
            {
                if (!came_from.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    came_from.Add(next,current);
                }
            }
        }
        return came_from;
    }

    public static List<Tile> BFSGetShortestPath(Tile start, Tile goal)
    {
        List<Tile> tilePath = new List<Tile>();
        var came_from = BFS(start, goal);
        Debug.Log(came_from.Count);

        var _current = goal;
        while(_current.gameObj != start.gameObj)
        {
            tilePath.Add(_current);
            _current = came_from[_current];
        }
        //tilePath.Add(start);
        return tilePath;
    }

    public static List<Tile> GetNeighboursInFourDirections(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        if(tile.X + 1 >=0 && tile.X + 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = TileMap.GetTileWithIndex(tile.X + 1, tile.Y);
            if (temp.envTileID != TileEnums.EnviromentTiles.level_1 && temp.envTileID != TileEnums.EnviromentTiles.level_2 || 
                temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.decID == TileEnums.Decorations.stairs)
            {
                neighbours.Add(temp);
            }
        }
        if(tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = TileMap.GetTileWithIndex(tile.X - 1, tile.Y);
            if (temp.envTileID != TileEnums.EnviromentTiles.level_1 && temp.envTileID != TileEnums.EnviromentTiles.level_2 ||
                temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.decID == TileEnums.Decorations.stairs)
            {
                neighbours.Add(temp);
            }
        }
        if (tile.X >= 0 && tile.X < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        {
            Tile temp = TileMap.GetTileWithIndex(tile.X, tile.Y + 1);
            if (temp.envTileID != TileEnums.EnviromentTiles.level_1 && temp.envTileID != TileEnums.EnviromentTiles.level_2 ||
                temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.decID == TileEnums.Decorations.stairs)
            {
                neighbours.Add(temp);
            }
        }
        if (tile.X >= 0 && tile.X < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        {
            Tile temp = TileMap.GetTileWithIndex(tile.X, tile.Y - 1);
            if (temp.envTileID != TileEnums.EnviromentTiles.level_1 && temp.envTileID != TileEnums.EnviromentTiles.level_2 ||
                temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.decID == TileEnums.Decorations.stairs)
            {
                neighbours.Add(temp);
            }
        }
        //if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        //{
        //    Tile temp = TileMap.GetTileWithIndex(tile.X + 1, tile.Y + 1);
        //    if (temp.envTileID != TileUtils.EnviromentTiles.level_1 && temp.envTileID != TileUtils.EnviromentTiles.level_2)
        //    {
        //        neighbours.Add(temp);
        //    }
        //}
        //if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        //{
        //    Tile temp = TileMap.GetTileWithIndex(tile.X + 1, tile.Y - 1);
        //    if (temp.envTileID != TileUtils.EnviromentTiles.level_1 && temp.envTileID != TileUtils.EnviromentTiles.level_2)
        //    {
        //        neighbours.Add(temp);
        //    }
        //}
        //if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y  + 1 >= 0 & tile.Y + 1 < 20)
        //{
        //    Tile temp = TileMap.GetTileWithIndex(tile.X - 1, tile.Y + 1);
        //    if (temp.envTileID != TileUtils.EnviromentTiles.level_1 && temp.envTileID != TileUtils.EnviromentTiles.level_2)
        //    {
        //        neighbours.Add(temp);
        //    }
        //}
        //if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        //{
        //    Tile temp = TileMap.GetTileWithIndex(tile.X - 1, tile.Y - 1);
        //    if (temp.envTileID != TileUtils.EnviromentTiles.level_1 && temp.envTileID != TileUtils.EnviromentTiles.level_2)
        //    {
        //        neighbours.Add(temp);
        //    }
        //}

        return neighbours;
    }
}
