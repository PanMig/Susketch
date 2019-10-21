using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/TileThemes")]
public class TileThemes : ScriptableObject
{
    public TileEnums.EnviromentTiles envTileID;
    public GameObject prefab;
    public Sprite sprite;
}


