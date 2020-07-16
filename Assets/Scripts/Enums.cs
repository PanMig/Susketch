﻿using System.Collections;
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
        RemoveOrPlace,
        randomMutation
    }

    public enum UIScreens
    {
        MapProperties,
        Predictions,
        Suggestions,
        Outro
    }

    public enum TutorialSessions
    {
        session_1,
        session_2,
        session_3,
        session_free
    }
}
