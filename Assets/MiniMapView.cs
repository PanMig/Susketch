using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bluePercentText;
    public TextMeshProUGUI BluePercentText => _bluePercentText;

    [SerializeField] private TextMeshProUGUI _redPercentText;
    public TextMeshProUGUI RedPercentText => _redPercentText;

    [SerializeField] private TextMeshProUGUI _resultText;
    public TextMeshProUGUI ResultsText => _resultText;

    [SerializeField] private TextMeshProUGUI _healthCountText;
    public TextMeshProUGUI HealthCountText => _healthCountText;

    [SerializeField] private TextMeshProUGUI _armorCountText;
    public TextMeshProUGUI ArmorCountText => _armorCountText;

    [SerializeField] private TextMeshProUGUI _dmgCountText;
    public TextMeshProUGUI DmgCountText => _dmgCountText;

    [SerializeField] private GameObject _btn;
    public GameObject Btn => _btn;

    [SerializeField] private GameObject _killRatioBar;
    public GameObject KillRatioBar => _killRatioBar;


    public void OnEnable()
    {
        MapSuggestionMng.onPickUpsGenerated += ActivateBtn;
    }

    public void OnDisable()
    {
        MapSuggestionMng.onPickUpsGenerated -= ActivateBtn;
    }

    public void Start()
    {
        _btn.SetActive(false);
        _bluePercentText.text = "Nan";
        _redPercentText.text = "Nan";
        _dmgCountText.text = "";
        _healthCountText.text = "";
        _armorCountText.text = "";
    }

    private void ActivateBtn(bool b)
    {
        _btn.SetActive(b);
    }

    public void SetKillRatioBar(float blueAmount, float redAmount)
    {
        var fillAmountBlue = _killRatioBar.transform.GetChild(0).GetComponent<Image>();
        var fillAmountRed = _killRatioBar.transform.GetChild(1).GetComponent<Image>();
        fillAmountBlue.fillAmount = blueAmount;
        fillAmountRed.fillAmount = redAmount;
    }
}
