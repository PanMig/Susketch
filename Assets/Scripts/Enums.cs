using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public static class Enums
{
    public enum classBalanceTypes
    {
        distinct,
        NoDistinct
    };

    public enum PowerUpPlacement
    {
        randomReplacement,
        regionSwap,
        modifyType,
        changePosition,
        RemoveOrPlace
    }

    public enum UIScreens
    {
        MapProperties,
        Predictions,
        Suggestions,
        Outro
    }
}
