using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSessionHandler : MonoBehaviour
{
    [SerializeField] private Enums.TutorialSessions _session;
    [SerializeField] private ModalWindowManager[] _sessionPanels;
    [SerializeField] private GameObject sessionWinBtn;

    public void SetState()
    {
        if (_session == Enums.TutorialSessions.free)
        {
            sessionWinBtn.SetActive(false);
        }
        else
        {
            _sessionPanels[(int)_session].OpenWindow();
            sessionWinBtn.SetActive(true);
        }
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
                _session = Enums.TutorialSessions.free;
                break;
        }
    }

    //public void EnableTutorialSessionPanel(int index)
    //{
    //    _sessionPanels[index].SetActive(true);
    //}
}
