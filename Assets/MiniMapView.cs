using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bluePercentText;
    public TextMeshProUGUI BluePercentText
    {
        get => _bluePercentText;
        set => _bluePercentText = value;
    }

    [SerializeField] private TextMeshProUGUI _redPercentText;
    public TextMeshProUGUI RedPercentText
    {
        get => _redPercentText;
        set => _redPercentText = value;
    }

    [SerializeField] private TextMeshProUGUI _resultText;
    public TextMeshProUGUI ResultsText
    {
        get => _resultText;
        set => _resultText = value;
    }

    [SerializeField] private GameObject _killRatioBar;

    [SerializeField] private GameObject _btn;
    public GameObject Btn
    {
        get => _btn;
        set => _btn.SetActive(value);
    }

    public void SetKillRatioBar(float blueAmount, float redAmount)
    {
        var fillAmountBlue = _killRatioBar.transform.GetChild(0).GetComponent<Image>();
        var fillAmountRed = _killRatioBar.transform.GetChild(1).GetComponent<Image>();
        fillAmountBlue.fillAmount = blueAmount;
        fillAmountRed.fillAmount = redAmount;
    }
}
