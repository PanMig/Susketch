using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TileMapLogic;
using UnityEngine;

public interface IPowerupPlacement
{
     Task<List<KeyValuePair<TileMap, float>>> ChangePowerUps(TileMap map);
}
