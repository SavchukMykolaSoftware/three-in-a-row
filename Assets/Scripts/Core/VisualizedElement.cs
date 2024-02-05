using UnityEngine;
using UnityEngine.UI;
using System;

public class VisualizedElement
{
    public GameObject ThisGameObject;
    public RectTransform ThisRectTransform;
    public UniformlyMovingUIObject ThisUniformlyMovingUIObject;
    public UniformlyScaledObject ThisUniformlyScaledObject;
    public AddingScoreAnimation ThisAddingScoreAnimation;
    public Button ThisButton;
    public int ColumnIndex;
    public int RowIndex;

    public VisualizedElement(GameObject thisGameObject, RectTransform thisRectTransform, UniformlyMovingUIObject thisUniformlyMovingUIObject, UniformlyScaledObject thisUniformlyScaledObject, AddingScoreAnimation thisAddingScoreAnimation, Button thisButton, int columnIndex, int rowIndex)
    {
        ThisGameObject = thisGameObject;
        ThisRectTransform = thisRectTransform;
        ThisUniformlyMovingUIObject = thisUniformlyMovingUIObject;
        ThisUniformlyScaledObject = thisUniformlyScaledObject;
        ThisAddingScoreAnimation = thisAddingScoreAnimation;
        ThisButton = thisButton;
        SetCoordinates(columnIndex, rowIndex);
        ThisButton.onClick.AddListener(SelectThisElement);
    }

    public void SetCoordinates(int newColumnIndex, int newRowIndex)
    {
        ColumnIndex = newColumnIndex;
        RowIndex = newRowIndex;
    }

    public void SelectThisElement()
    {
        if (BoardVisualizer.instance.IsShowingMoveNow == false)
        {
            GameLogic.instance.SelectElement(ColumnIndex, RowIndex);
        }
    }

    public void HighlightThisElement(Vector3 newSize, float scalingSpeed, Action afterFinishingScaling)
    {
        ThisUniformlyScaledObject.ChangeScale(newSize - ThisRectTransform.localScale, scalingSpeed, afterFinishingScaling);
    }
}