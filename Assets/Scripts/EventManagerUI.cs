using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerUI
{
    public delegate void OnSingleClickEdit();
    public static OnSingleClickEdit onSingleClickEdit;

    public delegate void OnDragClickEdit();
    public static OnDragClickEdit onDragClickEdit;

    public delegate void OnMapPlayable();
    public static OnMapPlayable onPlayableMap;

    public delegate void OnMapReadyForPrediction();
    public static OnMapReadyForPrediction onMapReadyForPrediction;

}