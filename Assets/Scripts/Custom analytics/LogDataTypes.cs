using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDataTypes
{

}

public class MainCanvasLog
{
    public string[,] map;
    public string blueClass;
    public string redClass;
    public string activeTab;

    public MainCanvasLog(string[,] map, string blueClass, string redClass, string activeTab)
    {
        this.map = map;
        this.blueClass = blueClass;
        this.redClass = redClass;
        this.activeTab = activeTab;
    }
}

