using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileMapLogic;
using static AuthoringTool;

public static class PathUtils
{
    public static bool DFS_Iterative(Tile start, Tile goal, TileMap tileMap)
    {
        Stack<Tile> frontier = new Stack<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        frontier.Push(start);
        while (frontier.Count > 0)
        {
            Tile v = frontier.Pop();
            if (!visited.Contains(v))
            {
                visited.Add(v);
                if (visited.Contains(goal))
                {
                    return true;
                }
                List<Tile> neighbours = GetNeighbours(v, tileMap);
                foreach (var neighbour in neighbours)
                {
                    frontier.Push(neighbour);
                }
            }
        }
        return false;
    }

    public static Dictionary<Tile, Tile> BFS(Tile start, Tile goal, TileMap tileMap)
    {
        Queue<Tile> frontier = new Queue<Tile>();
        frontier.Enqueue(start);

        Dictionary<Tile, Tile> came_from = new Dictionary<Tile, Tile>();
        came_from.Add(start, start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            List<Tile> neighbours = GetNeighbours(current, tileMap);
            foreach (var next in neighbours)
            {
                if (!came_from.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    came_from.Add(next, current);
                }
            }
        }
        return came_from;
    }

    public static List<Tile> BFSGetShortestPath(Tile start, Tile goal, TileMap tileMap)
    {
        List<Tile> tilePath = new List<Tile>();
        var came_from = BFS(start, goal, tileMap);

        var _current = goal;
        while (_current.gameObj != start.gameObj)
        {
            tilePath.Add(_current);
            _current = came_from[_current];
        }
        //tilePath.Add(start);
        return tilePath;
    }

    public static List<Tile> GetNeighbours(Tile tile, TileMap tileMap)
    {
        List<Tile> neighbours = new List<Tile>();

        if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X + 1, tile.Y);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X - 1, tile.Y);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X >= 0 && tile.X < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X, tile.Y + 1);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X >= 0 && tile.X < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X, tile.Y - 1);
            AddNeighbour(tile, neighbours, temp);
        }

        // extra four dimensions
        if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X + 1, tile.Y + 1);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X + 1, tile.Y - 1);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X - 1, tile.Y + 1);
            AddNeighbour(tile, neighbours, temp);
        }
        if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X - 1, tile.Y - 1);
            AddNeighbour(tile, neighbours, temp);
        }

        return neighbours;
    }

    private static void AddNeighbour(Tile tile, List<Tile> neighbours, Tile temp)
    {
        if (temp.envTileID != TileEnums.EnviromentTiles.level_1 && temp.envTileID != TileEnums.EnviromentTiles.level_2 ||
                        temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.decID == TileEnums.Decorations.stairs||
                        temp.envTileID == TileEnums.EnviromentTiles.level_1 && tile.envTileID == TileEnums.EnviromentTiles.level_1)
        {
            neighbours.Add(temp);
        }
    }

    public static List<Tile> GetNeighboursCross(Tile tile, TileMap tileMap)
    {
        List<Tile> neighbours = new List<Tile>();
        //down
        if (tile.X + 1 >= 0 && tile.X + 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X + 1, tile.Y);
            neighbours.Add(temp);
        }
        //up
        if (tile.X - 1 >= 0 && tile.X - 1 < 20 && tile.Y >= 0 & tile.Y < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X - 1, tile.Y);
            neighbours.Add(temp);
        }
        //right
        if (tile.X >= 0 && tile.X < 20 && tile.Y + 1 >= 0 & tile.Y + 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X, tile.Y + 1);
            neighbours.Add(temp);
        }
        //left
        if (tile.X >= 0 && tile.X < 20 && tile.Y - 1 >= 0 & tile.Y - 1 < 20)
        {
            Tile temp = tileMap.GetTileWithIndex(tile.X, tile.Y - 1);
            neighbours.Add(temp);
        }
        return neighbours;
    }

    public static List<Tile> RecursiveFloodFill(int posX, int posY, TileThemes tileID, List<Tile> tileList)
    {
        if ((posX < 0) || (posX >= TileMap.rows)) return tileList;
        if ((posY < 0) || (posY >= TileMap.columns)) return tileList;

        Tile tile = tileMapMain.GetTileWithIndex(posX, posY);
        if (tileList.Contains(tile)) // recursive call can lead to previously visited tiles.
        {
            return tileList;
        }
        if (tile.envTileID == tileID.envTileID)
        {
            tileList.Add(tile);
            RecursiveFloodFill(posX + 1, posY, tileID, tileList);
            RecursiveFloodFill(posX, posY + 1, tileID, tileList);
            RecursiveFloodFill(posX - 1, posY, tileID, tileList);
            RecursiveFloodFill(posX, posY - 1, tileID, tileList);
        }
        return tileList;
    }

    public static void FloodFill(int x, int y, int targetColor, int replaceColor, int[,] map)
    {
        if (x < 0 || x >= TileMap.rows
             || y < 0 || y >= TileMap.columns)
        {
            return;
        }
        else if (map[x, y] == replaceColor)
        {
            return;
        }

        if (map[x, y] == targetColor)
        {
            map[x, y] = replaceColor;
            FloodFill(x + 1, y, targetColor, replaceColor, map);
            FloodFill(x, y + 1, targetColor, replaceColor, map);
            FloodFill(x - 1, y, targetColor, replaceColor, map);
            FloodFill(x, y - 1, targetColor, replaceColor, map);
        }
    }
}
