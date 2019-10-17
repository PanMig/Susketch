using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Decoration", menuName = "ScriptableObjects/Decorations")]
public class Decoration : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;
    public TileUtils.Decorations decorationID;
}
