using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class MapPlayabilityButton : MonoBehaviour
{
    public Toggle toogle;
    private Animator anim;
    public TextMeshProUGUI modalWinText;

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

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void EnableButton()
    {
        // enable button
        gameObject.GetComponent<Image>().enabled = true;
        gameObject.GetComponent<Button>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        if (toogle != null)
        {
            toogle.GetComponent<Toggle>().isOn = false; // set toogle to NO
        }
        modalWinText.text = TileMapRepair.errorMsg;
    }

    void DisableButton()
    {
        // Disable button
        gameObject.GetComponent<Image>().enabled = false;
        gameObject.GetComponent<Button>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        if (toogle != null)
        {
            toogle.GetComponent<Toggle>().isOn = true; // set toogle to Yes
        }
    }

    // Update is called once per frame
    void SetButton()
    {

        if (!toogle.isOn)
        {
            gameObject.GetComponent<Image>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);

        }
        else
        {
            gameObject.GetComponent<Image>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
