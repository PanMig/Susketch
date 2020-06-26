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
    private AuthoringTool tool;

    public void Start()
    {
        tool = GetComponent<AuthoringTool>();
    }

    public void SaveMap()
    {
        AuthoringTool.tileMapMain.ExportTileMapToCSV(saveField.text);
    }

    public void LoadMap()
    {
        tool.LoadMap(loadField.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ScreenCapture.CaptureScreenshot("testCapt");
            }
        }
    }
}
