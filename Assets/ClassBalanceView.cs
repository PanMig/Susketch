using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum classBalanceType
{
    distinct,
    same
}

public class ClassBalanceView : MonoBehaviour
{
    // UI metrics
    [SerializeField] private classBalanceType type;
    [SerializeField] private GameObject _killRatioBar;
    [SerializeField] private TextMeshProUGUI _bluePercent;
    [SerializeField] private TextMeshProUGUI _redPercent;
    [SerializeField] private TextMeshProUGUI _classesText;
    [SerializeField] private GameObject _btn;
    [SerializeField] private TextMeshProUGUI _resultsText;

    private CharacterParams[] balancedClasses = new CharacterParams[2];

    private float _killRatio;
    public float KillRatio => _killRatio;

    private float _percentageError;
    public float PercentageError => _percentageError;

    public string ClassesText => _classesText.text;

    public bool wasApplied;

    public void OnEnable()
    {
        if (type == classBalanceType.distinct)
        {
            AuthoringTool.onclassBalanceDistinct += SetClassBalanceView;
        }
        else
        {
            AuthoringTool.onclassBalanceSame += SetClassBalanceView;
        }

        MapSuggestionMng.onCharactersBalanced += SetButtonState;
        _btn.GetComponent<Button>().onClick.AddListener(ApplySuggestedClasses);
    }

    private void SetButtonState(bool value)
    {
        _btn.SetActive(value);
    }

    public void OnDisable()
    {
        AuthoringTool.onclassBalanceDistinct -= SetClassBalanceView;
        AuthoringTool.onclassBalanceSame -= SetClassBalanceView;
    }

    private void SetClassBalanceView(KeyValuePair<CharacterParams[], float> balancedMatch)
    {
        wasApplied = false;
        _killRatio = balancedMatch.Value;
        _bluePercent.text = $"{((1 - _killRatio) * 100).ToString("F0")} %";
        _redPercent.text = $"{(_killRatio * 100).ToString("F0")} %";
        //blue is first player in input creation ONLY.
        balancedClasses[0] = balancedMatch.Key[0];
        //red is second.
        balancedClasses[1] = balancedMatch.Key[1];
        _classesText.text = $"{balancedClasses[0].className} vs {balancedClasses[1].className}";
        MetricsManager.SetKillRatioBar(1 - _killRatio, _killRatio, _killRatioBar);
        _percentageError = MetricsManager.CalculateRatioDifference(_killRatio, AuthoringTool.currKillRatio);
        if (_percentageError > 0)
        {
            _resultsText.text = $"+{_percentageError.ToString($"F0")} % gain";
            _resultsText.color = Color.green;
        }
        else
        {
            _resultsText.text = $"-{Mathf.Abs(_percentageError).ToString($"F0")} % loss";
            _resultsText.color = Color.red;
        }
        _btn.SetActive(true);
    }

    public void ApplySuggestedClasses()
    {
        CharacterClassMng.Instance.BlueClass = balancedClasses[0];
        CharacterClassMng.Instance.RedClass = balancedClasses[1];
        var blueIndex = CharacterClassMng.Instance.GetClassIndex(balancedClasses[0].className);
        var redIndex = CharacterClassMng.Instance.GetClassIndex(balancedClasses[1].className);
        CharacterClassMng.Instance.SetClassSprites(blueIndex, redIndex);
        CharacterClassMng.Instance.SetClassSpriteSelectors(blueIndex, redIndex);
        wasApplied = true;
        _btn.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _btn.SetActive(false);
        _classesText.text = "No input";
    }
}
