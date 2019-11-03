using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetricsManager : MonoBehaviour
{
    public DeathHeatmap heatmapInstance;

    public void GenerateDeathHeatmap(float[,] heatmap)
    {
        if(heatmapInstance.gameObject.activeInHierarchy == false)
        {
            heatmapInstance.DisplayDeathHeatmap(heatmap);
        }
        else
        {
            heatmapInstance.gameObject.SetActive(false);
        }
    }

    public void GenerateDramaticArcGraph(float[] dramatic_arc)
    {

    }

    public void GenerateCombatPaceGraph(float[] combat_pace)
    {

    }
}
