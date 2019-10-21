using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "FPS_Class", menuName = "ScriptableObjects/FPS_Classes")]
public class CharacterParams : ScriptableObject
{
    [Tooltip("HP, Speed, Dmg, Acc, Clip size, ROF, Bullets per shot, range")]
    public double[] class_params = new double[8];
}
