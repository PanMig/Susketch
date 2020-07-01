using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class OutroManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveField;
    [SerializeField] private TextMeshProUGUI loadField;
    [SerializeField] private TextMeshProUGUI loadFromMenuField;
    [SerializeField] private TextMeshProUGUI exportField;
    [SerializeField] private TextMeshProUGUI saveAndExitField;

    private AuthoringTool tool;

    public void Start()
    {
        tool = GetComponent<AuthoringTool>();
    }

    public void SaveMap()
    {
        if (saveField.text == "")
        {
            saveField.text = "myMap";
        }
        AuthoringTool.tileMapMain.ExportTileMapToCSV(saveField.text);
    }

    public void LoadMap()
    {
        try
        {
            tool.LoadMap(loadField.text);
        }
        catch (Exception e)
        {
            Debug.Log("failed");
        }
    }

    public void LoadFromMenu()
    {
        tool.LoadMap(loadFromMenuField.text);
    }

    public void ExportMetrics()
    {
        if (exportField.text == "")
        {
            exportField.text = "myMapMetrics";
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SaveAndExit()
    {
        if (saveAndExitField.text == "")
        {
            saveAndExitField.text = "myMap";
        }
        AuthoringTool.tileMapMain.ExportTileMapToCSV(saveAndExitField.text);
        Application.Quit();
    }
}
