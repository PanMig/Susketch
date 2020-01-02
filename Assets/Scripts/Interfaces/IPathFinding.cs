using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinding
{
    void FindShortestPath(Vector2 start, Vector2 goal, PlayerPathProperties playerProps);
}
