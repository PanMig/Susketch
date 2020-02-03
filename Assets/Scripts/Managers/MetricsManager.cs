using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using System.Linq;
using static MapSuggestionMng;
using System;
using TMPro;

public class MetricsManager : MonoBehaviour
{
    public DeathHeatmap heatmapOverlay;
    public DeathHeatmap heatmapUI;
    public LineChartCtrl daChart;
    public LineChartCtrl cpChart;
    public ProgressBar killRatioBar;
    public TextMeshProUGUI GameDurationText;
    public Button classBalanceBtn;
    public Button pickUpsBalanceBtn;

    private void OnEnable()
    {
        OnBalancedCharacters += SetClassBalanceBtn;
        OnGeneratedPickUps += SetPickupsBtn;
    }

    private void OnDisable()
    {
        OnBalancedCharacters -= SetClassBalanceBtn;
        OnGeneratedPickUps -= SetPickupsBtn;
    }

    public void Start()
    {
        classBalanceBtn.interactable = false;
        pickUpsBalanceBtn.interactable = false;
    }

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
        for (int i = 0; i < dramatic_arc.Length; i++)
        {
            dramatic_arc[i] = -dramatic_arc[i];
        }
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

    public void SetGameDurationText(float value)
    {
        if (value < 0.28f)
        {
            GameDurationText.text = "Short";
        }
        else if (value >= 0.28f && value < 0.43f)
        {
            GameDurationText.text = "Medium";
        }
        else
        {
            GameDurationText.text = "Long";
        }
    }

    public void SetPickupsBtn(bool value)
    {
        pickUpsBalanceBtn.interactable = value;
    }

    public void SetClassBalanceBtn(bool value)
    {
        classBalanceBtn.interactable = value;
    }
}
