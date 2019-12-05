using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI;

public class MapPlayabilityButton : MonoBehaviour
{
    public Toggle toogle;
    private Animator anim;

    public void OnEnable()
    {
        EventManagerUI.onTileMapEdit += SetButton;
    }

    public void OnDisable()
    {
        EventManagerUI.onTileMapEdit -= SetButton;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
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
