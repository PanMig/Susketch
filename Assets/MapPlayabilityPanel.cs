using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPlayabilityPanel : MonoBehaviour
{
    public GameObject panel;

    public void OnEnable()
    {
        TileMapRepair.onPlayableMap += DisableButton;
        TileMapRepair.onUnPlayableMap += EnableButton;
    }

    public void OnDisable()
    {
        TileMapRepair.onPlayableMap -= DisableButton;
        TileMapRepair.onUnPlayableMap -= EnableButton;
    }

    void EnableButton()
    {
        // enable button
        panel.GetComponent<Image>().enabled = true;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(true);
        }
    }

    void DisableButton()
    {
        // Disable button
        panel.GetComponent<Image>().enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

    }
}
