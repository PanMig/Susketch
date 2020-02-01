using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushButtons : MonoBehaviour
{
    [SerializeField] private Button[] tilebrushes;
    [SerializeField] private Button[] decorbrushes;
    [SerializeField] private Button[] fillbrushes;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SelectBrush();
    }

    private void SelectBrush()
    {
        
        if(TileCursor.currentCursorType == TileCursor.CursorType.tile)
        {
            index = Brush.Instance.currTileBrush;
            SelectButton(tilebrushes, index);
        }
        else if (TileCursor.currentCursorType == TileCursor.CursorType.decoration)
        {
            index = Brush.Instance.currDecBrush;
            SelectButton(decorbrushes, index);
        }
        else
        {
            index = Brush.Instance.currTileBrush;
            SelectButton(fillbrushes, index);
        }

    }

    private void SelectButton(Button[] buttons, int index)
    {
        buttons[index].Select();
        buttons[index].OnSelect(null);
    }
}
