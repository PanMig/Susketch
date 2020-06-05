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
    [SerializeField] private classBalanceType type;
    [SerializeField] private GameObject _killRatioBar;
    [SerializeField] private TextMeshProUGUI _bluePercent;
    [SerializeField] private TextMeshProUGUI _redPercent;
    [SerializeField] private TextMeshProUGUI _classesText;
    [SerializeField] private GameObject _btn;
    [SerializeField] private TextMeshProUGUI _resultsText;
    private CharacterParams[] balancedClasses = new CharacterParams[2]; 

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

    private void SetClassBalanceView(KeyValuePair<CharacterParams[], float> balancedmatch)
    {
        var percent = balancedmatch.Value;
        _bluePercent.text = $"{((1 - percent) * 100).ToString("F0")} %";
        _redPercent.text = $"{(percent * 100).ToString("F0")} %";
        //blue is first player in input creation ONLY.
        balancedClasses[0] = balancedmatch.Key[0];
        //red is second.
        balancedClasses[1] = balancedmatch.Key[1];
        _classesText.text = $"{balancedClasses[0].className} vs {balancedClasses[1].className}";
        MetricsManager.SetKillRatioBar(1 - percent, percent, _killRatioBar);
        var result = MetricsManager.CalculateRatioDifference(percent, AuthoringTool.currKillRatio);
        if (result < 0)
        {
            _resultsText.text = $"+{Mathf.Abs(result).ToString($"F0")} % gain";
            _resultsText.color = Color.green;
        }
        else
        {
            _resultsText.text = $"-{Mathf.Abs(result).ToString($"F0")} % loss";
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
        _btn.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _btn.SetActive(false);
        _classesText.text = "No input";
    }
}
