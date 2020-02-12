using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerUI
{
    public delegate void OnTileMapEdit();
    public static OnTileMapEdit onTileMapEdit;

    public delegate void OnMapPlayable();
    public static OnTileMapEdit onPlayableMap;

    public delegate void OnMapReadyForPrediction();
    public static OnMapReadyForPrediction onMapReadyForPrediction;
}