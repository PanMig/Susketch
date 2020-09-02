using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSessionHandler : MonoBehaviour
{
    [SerializeField] private Enums.TutorialSessions _session;
    public Enums.TutorialSessions Session => _session;

    [SerializeField] private ModalWindowManager[] _sessionPanels;
    [SerializeField] private GameObject sessionWinBtn;

    //Menu items.
    [SerializeField] private Button _newMapBtn;
    [SerializeField] private Button _loadMapBtn;
    [SerializeField] private Button _sessOneBtn;
    [SerializeField] private Button _sessTwoBtn;
    [SerializeField] private Button _sessThreeBtn;

    public enum LearningState
    {
        firstEntry,
        oneSessComplete,
        twoSessComplete,
        threeSessComplete
    }
    public LearningState LearnState { get; set; }

    void Awake()
    {
        SetButtonsState();
        //foreach (var panel in _sessionPanels)
        //{
        //    panel.gameObject.SetActive(false);
        //}
    }

    public void SetState()
    {
        if (_session == Enums.TutorialSessions.session_free)
        {
            sessionWinBtn.SetActive(false);
            foreach (var panel in _sessionPanels)
            {
                panel.CloseWindow();
            }
        }
        else
        {
            _sessionPanels[(int)_session].OpenWindow();
            sessionWinBtn.SetActive(true);
        }
        UserLogsBuilder.InitFileNameSettings(_session);
    }

    public void SetTutorialSession(int index)
    {
        switch (index)
        {
            case 1:
                _session = Enums.TutorialSessions.session_1;
                break;
            case 2:
                _session = Enums.TutorialSessions.session_2;
                break;
            case 3:
                _session = Enums.TutorialSessions.session_3;
                break;
            default:
                _session = Enums.TutorialSessions.session_free;
                break;
        }
    }

    public void SetLearningState(int index)
    {
        switch (index)
        {
            case 1:
                LearnState = LearningState.oneSessComplete;
                PlayerPrefs.SetString("LearningState", LearnState.ToString());
                break;
            case 2:
                LearnState = LearningState.twoSessComplete;
                PlayerPrefs.SetString("LearningState", LearnState.ToString());
                break;
            case 3:
                LearnState = LearningState.threeSessComplete;
                PlayerPrefs.SetString("LearningState", LearnState.ToString());
                break;
            default:
                LearnState = LearningState.firstEntry;
                PlayerPrefs.SetString("LearningState", LearnState.ToString());
                break;
        }
        SetButtonsState();
    }

    public void SetButtonsState()
    {
        var state = PlayerPrefs.GetString("LearningState");
        if (state == string.Empty)
        {
            state = LearningState.firstEntry.ToString();
        }

        if (state == LearningState.firstEntry.ToString())
        {
            _newMapBtn.interactable = false;
            _loadMapBtn.interactable = false;
            _sessOneBtn.interactable = true;
            _sessTwoBtn.interactable = false;
            _sessThreeBtn.interactable = false;
        }
        else if (state == LearningState.oneSessComplete.ToString())
        {
            _newMapBtn.interactable = false;
            _loadMapBtn.interactable = false;
            _sessOneBtn.interactable = true;
            _sessTwoBtn.interactable = true;
            _sessThreeBtn.interactable = false;
        }
        else if (state == LearningState.twoSessComplete.ToString())
        {
            _newMapBtn.interactable = false;
            _loadMapBtn.interactable = false;
            _sessOneBtn.interactable = true;
            _sessTwoBtn.interactable = true;
            _sessThreeBtn.interactable = true;
        }
        else if (state == LearningState.threeSessComplete.ToString())
        {
            _newMapBtn.interactable = true;
            _loadMapBtn.interactable = true;
            _sessOneBtn.interactable = true;
            _sessTwoBtn.interactable = true;
            _sessThreeBtn.interactable = true;
        }
    }
}
