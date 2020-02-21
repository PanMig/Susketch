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
    public GameObject killRatioBar;
    public TextMeshProUGUI killRatioTextBlue;
    public TextMeshProUGUI killRatioTextRed;
    public TextMeshProUGUI GameDurationText;
    public Image GameDurationRadialBar;
    public Button classBalanceBtn;
    public Button pickUpsBalanceBtn;

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
        cpChart.SetChartData(combat_pace);
    }

    public void SetKillRatioProgressBar(float percent)
    {
        var fillAmountBlue = killRatioBar.transform.GetChild(0).GetComponent<Image>();
        var fillAmountRed = killRatioBar.transform.GetChild(1).GetComponent<Image>();
        float blueAmount = 1 - percent;
        float redAmount = percent;
        fillAmountBlue.fillAmount = blueAmount;
        fillAmountRed.fillAmount = redAmount;
        killRatioTextBlue.text = Mathf.Floor(blueAmount * 100).ToString() + "%";
        killRatioTextRed.text =  Mathf.Floor(redAmount * 100).ToString()  + "%";
    }

    public void SetGameDurationText(float value)
    {
        //reverse min max normalized value.
        float timeSecs = (value * 450) + 150; 
        if (value < 0.28f)
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F1") + " / 10 min \n (Short)";
        }
        else if (value >= 0.28f && value < 0.43f)
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F1") + "/ 10 min \n (Medioum)";
        }
        else
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F1") + "/ 10 min \n (Long)";
        }
        GameDurationRadialBar.fillAmount = timeSecs / 600.0f;
    }
}
