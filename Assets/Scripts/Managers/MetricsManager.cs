﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class MetricsManager : MonoBehaviour
{
    public DeathHeatmap heatmapOverlay;
    public DeathHeatmap heatmapUI;
    public LineChartCtrl daChart;
    public LineChartCtrl cpChart;
    public ProgressBar killRatioBar;

    public void DeathHeatmapButtonListener(float[,] heatmap)
    {
        if(heatmapOverlay.gameObject.activeInHierarchy == false)
        {
            heatmapOverlay.UpdateDeathHeatmap(heatmap);
            heatmapOverlay.EnableHeatmap();
        }
        else
        {
            heatmapOverlay.gameObject.SetActive(false);
        }
    }

    public void GenerateDeathHeatmap(float[,] heatmap)
    {
        heatmapUI.UpdateDeathHeatmap(heatmap);
        if (!heatmapUI.gameObject.activeInHierarchy)
        {
            heatmapUI.EnableHeatmap();
        }
    }

    public void GenerateDramaticArcGraph(float[] dramatic_arc)
    {
        daChart.SetChartData(dramatic_arc);
    }

    public void GenerateCombatPaceGraph(float[] combat_pace)
    {

    }

    public void SetKillRatioProgressBar(float percent)
    {
        killRatioBar.currentPercent = percent;
        var child = killRatioBar.transform.GetChild(0);
        var loadingBar = child.GetChild(0).GetComponent<Image>();
        if(percent > 60.0f)
        {
            loadingBar.color = Color.red;
        }
        else if(percent < 40.0f)
        {
            loadingBar.color = Color.blue;
        }
        else
        {
            loadingBar.color = new Color32(255,133,0,255);
        }
    }
}