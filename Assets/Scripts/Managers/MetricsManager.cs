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
        //classBalanceBtn.interactable = false;
        //pickUpsBalanceBtn.interactable = false;
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
        var blueAmount = (1 - percent) * 100.0f;
        var redAmount = percent* 100.0f;
        fillAmountBlue.fillAmount = blueAmount / 100.0f;
        fillAmountRed.fillAmount = redAmount / 100.0f;
        killRatioTextBlue.text = $"{blueAmount.ToString("F0")} %";
        killRatioTextRed.text =  $"{redAmount.ToString("F0")} %";
    }

    public void SetGameDurationText(float value)
    {
        //reverse min max normalized value.
        float timeSecs = (value * 450) + 150; 
        if (value < 0.28f)
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F0") + " min (Short)";
        }
        else if (value >= 0.28f && value < 0.43f)
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F0") + "  min (Medium)";
        }
        else
        {
            GameDurationText.text = (Mathf.Floor(timeSecs) / 60.0f).ToString("F0") + "  min (Long)";
        }

        // total amount of match duration is 600 secs.
        GameDurationRadialBar.fillAmount = timeSecs / 600.0f;
    }

    public static void SetKillRatioBar(float blueAmount, float redAmount, GameObject killRatioBar)
    {
        var fillAmountBlue = killRatioBar.transform.GetChild(0).GetComponent<Image>();
        var fillAmountRed = killRatioBar.transform.GetChild(1).GetComponent<Image>();
        fillAmountBlue.fillAmount = blueAmount;
        fillAmountRed.fillAmount = redAmount;
    }

    // When error is negative we are closer to the desired value.
    public static float CalculateRatioDifference(float newPercent, float curPercent)
    {
        // we use absolute values for cases were KR is below 0.5 and their difference is positive.
        var suggestedKR = Mathf.Abs(0.5f - newPercent);
        var currentKR = Mathf.Abs(0.5f - curPercent);
        if (currentKR == 0.0f) return 0.0f;
        // we divide by the desired number(0.5) so we get the percentage error (mathematical measurement).
        var result = (currentKR - suggestedKR) / 0.5f;
        return result * 100;
    }
}
