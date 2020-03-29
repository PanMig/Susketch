using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite _defaultColor;
    [SerializeField] private Sprite _eraseColor;
    private Image _highlightImg;

    void Start()
    {
        try
        {
            _highlightImg = transform.GetChild(1).GetComponent<Image>();
            _highlightImg.sprite = _defaultColor;
        }
        catch (Exception e)
        {
            Debug.LogError("No highlight image child was found in default tile.");
        }
        _highlightImg.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TileCursor.currentCursorType == TileCursor.CursorType.decoration && Brush.Instance.currDecBrush == 0)
        {
            _highlightImg.sprite = _eraseColor;
        }
        _highlightImg.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highlightImg.enabled = false;
        _highlightImg.sprite = _defaultColor;
    }

    public void ResizeImage(float removePercent, Image img)
    {
        img.transform.GetComponent<RectTransform>().sizeDelta =
            new Vector2(GetComponent<RectTransform>().sizeDelta.x * removePercent, GetComponent<RectTransform>().sizeDelta.y * removePercent);
    }
}
