using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovementStep : MonoBehaviour
{
    public TextMeshProUGUI textNumber;
    public int team;

    private void OnEnable()
    {
        //EventManagerUI.onPlayableMap += SetText;
        EventManagerUI.onSingleClickEdit += SetText;
    }

    private void OnDisable()
    {
        //EventManagerUI.onPlayableMap -= SetText;
        EventManagerUI.onSingleClickEdit -= SetText;
    }

    private void Start()
    {
        SetText();
    }

    public void SetText()
    {
        PathManager.Instance.UpdateMovementSteps();
        if (team == 0)
        {
            textNumber.text = PathManager.Instance.pathRedProps.movementSteps.ToString();
        }
        else if(team == 1)
        {
            textNumber.text = PathManager.Instance.pathBlueProps.movementSteps.ToString();
        }
        else
        {
            textNumber.text = "Nan";
        }
    }

    
}
