using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuggestiveReplacementButton : MonoBehaviour
{
    public enum ReplacementType { classBalance, pickUpPlacementKR, pickUpPlacementDA }
    public ReplacementType replacementType;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        RegisterEvents();
    }

    public void RegisterEvents()
    {
        switch (replacementType)
        {
            case ReplacementType.classBalance:
                MapSuggestionMng.onCharactersBalanced += ActivateButton;
                break;
            case ReplacementType.pickUpPlacementKR:
                MapSuggestionMng.onPickUpsGenerated += ActivateButton;
                break;
            default:
                break;
        }
    }

    private void ActivateButton(bool value)
    {
        if (value)
        {
            animator.SetBool("highlight", true);
        }
        else
        {
            animator.SetBool("highlight", false);
        }
    }
}
