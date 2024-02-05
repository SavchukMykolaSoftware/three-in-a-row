using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardVisualizer : MonoBehaviour
{
    [SerializeField] private ElementsRegister m_AllPossibleElements;
    [SerializeField, Range(0, 1)] private float m_MarginOfOneElement = 0.2f;
    [SerializeField, Range(0, 0.5f)] private float m_BoardHorizontalMargin = 0.1f;
    [SerializeField, Range(0, 0.5f)] private float m_BoardVerticalMargin = 0.1f;
    [SerializeField] private GameObject m_OneElementSample;
    [SerializeField] private RectTransform m_ParentForAllElements;
    [SerializeField] private float m_SwappingSpeedCoefficient = 3;
    [SerializeField] private float m_FallingSpeedCoefficient = 4;
    [SerializeField] private float m_ScalingSpeedWhenDisappearingAfterMove = 10;
    [SerializeField] private float m_ScalingSpeedWhenUpdatingPosition = 5;
    [SerializeField] private float m_ScalingSpeedWhenShowingSelection = 1;
    [SerializeField] private float m_ElementSizeWhenShowingSelection = 0.9f;

    [Header("Optional")]
    [SerializeField] private RectTransform m_TargetForAddingScoreAnimation;
    [SerializeField] private Canvas m_CanvasWithTargetForAddingScoreAnimation;

    private Dictionary<ElementID, PossibleElementData> m_ElementsWithTheirID;
    private float m_OneElementLength;
    private float m_OneElementHeight;
    private int m_QuantityOfElementsInOneRow;
    private int m_QuantityOfElementsInOneColumn;
    private Vector3 m_LeftBottomAngleElementPosition;
    private float m_SwappingSpeedInPixels;
    private float m_FallingSpeedInPixels;
    private Vector3 m_UsualScaleOfOneElement;
    private Vector3 m_ScaleOfOneHighlightedElement;
    private Vector2 m_PositionOfTargetForAddingScoreAnimation;

    public VisualizedElement[,] ObjectsWhichShowPosition { get; private set; }

    public static BoardVisualizer instance;

    private bool m_IsShowingMoveNow = false;
    public bool IsShowingMoveNow
    {
        get
        {
            return m_IsShowingMoveNow;
        }
        set
        {
            m_IsShowingMoveNow = value;

        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void InitializeBoardParameters(int quantityOfElementsInOneRow, int quantityOfElementsInOneColumn)
    {
        m_QuantityOfElementsInOneRow = quantityOfElementsInOneRow;
        m_QuantityOfElementsInOneColumn = quantityOfElementsInOneColumn;
        ObjectsWhichShowPosition = new VisualizedElement[m_QuantityOfElementsInOneRow, m_QuantityOfElementsInOneColumn];
        m_ElementsWithTheirID = m_AllPossibleElements.GetElementsWithTheirID();

        m_OneElementLength = ((1 - 2 * m_BoardHorizontalMargin) * m_ParentForAllElements.rect.width) / (m_QuantityOfElementsInOneRow);
        m_OneElementHeight = ((1 - 2 * m_BoardVerticalMargin) * m_ParentForAllElements.rect.height) / (m_QuantityOfElementsInOneColumn);
        Vector3 OneElementScale = Vector3.one;

        m_SwappingSpeedInPixels = m_OneElementLength * m_SwappingSpeedCoefficient;
        m_FallingSpeedInPixels = m_OneElementLength * m_FallingSpeedCoefficient;

        m_LeftBottomAngleElementPosition = new Vector3(-((1 - 2 * m_BoardHorizontalMargin) * m_ParentForAllElements.rect.width) / 2 + m_OneElementLength / 2 + m_MarginOfOneElement, -((1 - 2 * m_BoardVerticalMargin) * m_ParentForAllElements.rect.height) / 2 + m_OneElementHeight / 2 + m_MarginOfOneElement);
        m_UsualScaleOfOneElement = OneElementScale;
        m_ScaleOfOneHighlightedElement = m_UsualScaleOfOneElement * m_ElementSizeWhenShowingSelection;
    
        if(m_TargetForAddingScoreAnimation != null && m_CanvasWithTargetForAddingScoreAnimation != null)
        {
            m_PositionOfTargetForAddingScoreAnimation = RectTransformUtility.WorldToScreenPoint(m_CanvasWithTargetForAddingScoreAnimation.worldCamera, m_TargetForAddingScoreAnimation.position);
        }
    }

    public void ShowPositionFromZero(ElementID[,] positionToBeShowed, bool shouldAppearWithAnimation, Action afterAnimation = null)
    {
        if (positionToBeShowed.GetLength(0) != m_QuantityOfElementsInOneRow || positionToBeShowed.GetLength(1) != m_QuantityOfElementsInOneColumn)
        {
            Debug.LogError("Error showing board: size of the arrays must be the same!");
        }
        else
        {
            Vector3 CurrentPositionOnTheBoard = m_LeftBottomAngleElementPosition;
            for (int i = 0; i < m_QuantityOfElementsInOneRow; i++)
            {
                for (int j = 0; j < m_QuantityOfElementsInOneColumn; j++)
                {
                    InstantiateNewElement(i, j, positionToBeShowed[i, j], CurrentPositionOnTheBoard, shouldAppearWithAnimation, m_ScalingSpeedWhenUpdatingPosition, afterAnimation);
                    CurrentPositionOnTheBoard.y += m_OneElementHeight + m_MarginOfOneElement;
                }
                CurrentPositionOnTheBoard.x += m_OneElementLength + m_MarginOfOneElement;
                CurrentPositionOnTheBoard.y = m_LeftBottomAngleElementPosition.y;
            }
        }
    }

    private Vector2 GetAnchoredPositionOfElement(int columnIndex, int rowIndex)
    {
        return (Vector2)m_LeftBottomAngleElementPosition + new Vector2(columnIndex * (m_OneElementLength + m_MarginOfOneElement), rowIndex * (m_OneElementHeight + m_MarginOfOneElement));
    }

    private void InstantiateNewElement(int columnIndex, int rowIndex, ElementID idOfNewElement, bool shouldAppearWithScaleAnimation, float scalingSpeed, Action afterAnimation = null)
    {
        Vector2 PositionInTheScene = GetAnchoredPositionOfElement(columnIndex, rowIndex);
        InstantiateNewElement(columnIndex, rowIndex, idOfNewElement, PositionInTheScene, shouldAppearWithScaleAnimation, scalingSpeed, afterAnimation);
    }

    private void InstantiateNewElement(int columnIndex, int rowIndex, ElementID idOfNewElement, Vector2 positionInTheScene, bool shouldAppearWithScaleAnimation, float scalingSpeed, Action afterAnimation = null)
    {
        GameObject NewElement = Instantiate(m_OneElementSample);
        RectTransform RectTransformOfNewElement = NewElement.GetComponent<RectTransform>();
        RectTransformOfNewElement.SetParent(m_ParentForAllElements);
        RectTransformOfNewElement.anchorMin = 0.5f * Vector2.one;
        RectTransformOfNewElement.anchorMax = 0.5f * Vector2.one;
        RectTransformOfNewElement.sizeDelta = (1 - m_MarginOfOneElement) * new Vector2(m_OneElementLength, m_OneElementHeight);
        RectTransformOfNewElement.anchoredPosition = positionInTheScene;
        RectTransformOfNewElement.localPosition = new Vector3(NewElement.transform.localPosition.x, NewElement.transform.localPosition.y, 0);
        RectTransformOfNewElement.localScale = shouldAppearWithScaleAnimation ? Vector3.zero : m_UsualScaleOfOneElement;
        Image ImageOfNewElement = NewElement.GetComponent<Image>();
        ImageOfNewElement.sprite = m_ElementsWithTheirID[idOfNewElement].Sprite;
        ImageOfNewElement.color = m_ElementsWithTheirID[idOfNewElement].Color;
        UniformlyScaledObject NewScaledObject = NewElement.GetComponent<UniformlyScaledObject>();
        if (shouldAppearWithScaleAnimation)
        {
            NewScaledObject.ChangeScale(m_UsualScaleOfOneElement, scalingSpeed, afterAnimation);
        }
        ObjectsWhichShowPosition[columnIndex, rowIndex] = new(NewElement, RectTransformOfNewElement, NewElement.GetComponent<UniformlyMovingUIObject>(), NewScaledObject, NewElement.GetComponent<AddingScoreAnimation>(), NewElement.GetComponent<Button>(), columnIndex, rowIndex);
    }

    public void HighlightElement(int columnIndex, int rowIndex, Action afterHighlighting = null)
    {
        ObjectsWhichShowPosition[columnIndex, rowIndex].HighlightThisElement(m_ScaleOfOneHighlightedElement, m_ScalingSpeedWhenShowingSelection, afterHighlighting);
    }

    public void UnhighlightElement(int columnIndex, int rowIndex, Action afterUnighlighting = null)
    {
        ObjectsWhichShowPosition[columnIndex, rowIndex].HighlightThisElement(m_UsualScaleOfOneElement, m_ScalingSpeedWhenShowingSelection, afterUnighlighting);
    }

    public void PlaceOneElementToPlaceOfAnother(int columnIndex1, int rowIndex1, int columnIndex2, int rowIndex2, float speed, Action afterPlacing = null)
    {
        ObjectsWhichShowPosition[columnIndex1, rowIndex1].SetCoordinates(columnIndex2, rowIndex2);
        ObjectsWhichShowPosition[columnIndex1, rowIndex1].ThisUniformlyMovingUIObject.Move(GetAnchoredPositionOfElement(columnIndex2, rowIndex2) - ObjectsWhichShowPosition[columnIndex1, rowIndex1].ThisRectTransform.anchoredPosition, speed, afterPlacing);
    }

    public void SwapTwoElements(int columnIndex1, int rowIndex1, int columnIndex2, int rowIndex2, Action afterSwapping = null)
    {
        PlaceOneElementToPlaceOfAnother(columnIndex1, rowIndex1, columnIndex2, rowIndex2, m_SwappingSpeedInPixels);
        PlaceOneElementToPlaceOfAnother(columnIndex2, rowIndex2, columnIndex1, rowIndex1, m_SwappingSpeedInPixels, afterSwapping);
        (ObjectsWhichShowPosition[columnIndex1, rowIndex1], ObjectsWhichShowPosition[columnIndex2, rowIndex2]) = (ObjectsWhichShowPosition[columnIndex2, rowIndex2], ObjectsWhichShowPosition[columnIndex1, rowIndex1]);
    }

    public void SwapTwoElementsWithReturning(int columnIndex1, int rowIndex1, int columnIndex2, int rowIndex2, Action afterReturning = null)
    {
        void MoveElementWithReturning(UniformlyMovingUIObject elementToBeMoved, Vector2 displacement, Action afterReturning = null)
        {
            elementToBeMoved.Move(displacement, m_SwappingSpeedInPixels, () =>
            {
                elementToBeMoved.Move(-displacement, m_SwappingSpeedInPixels, afterReturning);
            });
        }

        Vector2 DisplacementFrom2To1 = ObjectsWhichShowPosition[columnIndex1, rowIndex1].ThisRectTransform.anchoredPosition - ObjectsWhichShowPosition[columnIndex2, rowIndex2].ThisRectTransform.anchoredPosition;
        MoveElementWithReturning(ObjectsWhichShowPosition[columnIndex1, rowIndex1].ThisUniformlyMovingUIObject, -DisplacementFrom2To1);
        MoveElementWithReturning(ObjectsWhichShowPosition[columnIndex2, rowIndex2].ThisUniformlyMovingUIObject, DisplacementFrom2To1, afterReturning);
    }

    public void DestroyOneVisualElement(int columnIndex, int rowIndex, float scalingSpeed, Action afterDestroying = null)
    {
        ObjectsWhichShowPosition[columnIndex, rowIndex].ThisUniformlyScaledObject.ChangeScale(-m_UsualScaleOfOneElement, scalingSpeed, () =>
        {
            Destroy(ObjectsWhichShowPosition[columnIndex, rowIndex].ThisGameObject);
            afterDestroying?.Invoke();
        });
    }

    public void DestroyAllVisualElements(Action afterDestroying = null)
    {
        int QuantityOfElements = m_QuantityOfElementsInOneColumn * m_QuantityOfElementsInOneRow;
        int CurrentQuantityOfDestroyedElements = 0;
        Action AfterDestroyingOneElement = () =>
        {
            CurrentQuantityOfDestroyedElements++;
            if (CurrentQuantityOfDestroyedElements == QuantityOfElements)
            {
                afterDestroying?.Invoke();
            }
        };
        for (int i = 0; i < m_QuantityOfElementsInOneRow; i++)
        {
            for (int j = 0; j < m_QuantityOfElementsInOneColumn; j++)
            {
                DestroyOneVisualElement(i, j, m_ScalingSpeedWhenUpdatingPosition, AfterDestroyingOneElement);
            }
        }
    }

    public void DestroyAndReplaceElements(List<KeyValuePair<int, List<KeyValuePair<int, ElementID>>>> elementsToBeDestroyedAndReplacedInColumns, Action<List<Vector2Int>> checkForNextMove = null, Action<AddingScoreAnimation, Vector2> showingScoreAnimation = null)
    {
        bool GetIfElementsWillFallInOneColumn(KeyValuePair<int, List<KeyValuePair<int, ElementID>>> column)
        {
            if (column.Value[^1].Key != m_QuantityOfElementsInOneColumn - 1)
            {
                return true;
            }
            for (int i = column.Value.Count - 2; i >= 0; i--)
            {
                if (column.Value[i + 1].Key - column.Value[i].Key != 1)
                {
                    return true;
                }
            }
            return false;
        }

        Dictionary<int, bool> DataIfElementsInColumnsWillFall = new();
        int QuantityOfColumnsWithFallingElements = 0;
        foreach (KeyValuePair<int, List<KeyValuePair<int, ElementID>>> oneColumnData in elementsToBeDestroyedAndReplacedInColumns)
        {
            bool WillElementsFallInThisColumn = GetIfElementsWillFallInOneColumn(oneColumnData);
            DataIfElementsInColumnsWillFall.Add(oneColumnData.Key, WillElementsFallInThisColumn);
            if(WillElementsFallInThisColumn)
            {
                QuantityOfColumnsWithFallingElements++;
            }
        }
        int QuantityOfColumnsWithoutFallingElements = elementsToBeDestroyedAndReplacedInColumns.Count - QuantityOfColumnsWithFallingElements;
        List<Vector2Int> PositionsOfElementsToBeChecked = new();
        Action AfterFalling = null;
        int CurrentQuantityOfColumnsWithFallenElements = 0;
        int CurrentQuantityOfReplacedTopElements = 0;
        foreach (KeyValuePair<int, List<KeyValuePair<int, ElementID>>> oneColumnData in elementsToBeDestroyedAndReplacedInColumns)
        {
            List<int> RowIndexesOfInstantiatedElementsInThisColumn = new();
            bool WillElementsFallInThisColumn = GetIfElementsWillFallInOneColumn(oneColumnData);
            int MinimalRowIndex = oneColumnData.Value[0].Key;
            int MaximalRowIndex = oneColumnData.Value[^1].Key;
            int CurrentQuantityOfDestroyedElementsInThisColumn = 0;
            List<KeyValuePair<int, ElementID>> TopElementsInThisColumn = new();
            if (MaximalRowIndex + 1 == m_QuantityOfElementsInOneColumn)
            {
                TopElementsInThisColumn.Add(oneColumnData.Value[^1]);
                for (int i = oneColumnData.Value.Count - 2; i >= 0; i--)
                {
                    if (oneColumnData.Value[i + 1].Key - oneColumnData.Value[i].Key == 1)
                    {
                        TopElementsInThisColumn.Add(oneColumnData.Value[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            foreach (KeyValuePair<int, ElementID> oneElementData in oneColumnData.Value)
            {
                if (ObjectsWhichShowPosition[oneColumnData.Key, oneElementData.Key].ThisAddingScoreAnimation != null)
                {
                    showingScoreAnimation?.Invoke(ObjectsWhichShowPosition[oneColumnData.Key, oneElementData.Key].ThisAddingScoreAnimation, m_PositionOfTargetForAddingScoreAnimation);
                }
                Action AfterScaling = () =>
                {
                    Destroy(ObjectsWhichShowPosition[oneColumnData.Key, oneElementData.Key].ThisGameObject);
                    CurrentQuantityOfDestroyedElementsInThisColumn += 1;
                };
                if (MaximalRowIndex + 1 == m_QuantityOfElementsInOneColumn)
                {
                    if (WillElementsFallInThisColumn == false)
                    {
                        AfterScaling += () =>
                        {
                            if (CurrentQuantityOfDestroyedElementsInThisColumn == TopElementsInThisColumn.Count)
                            {
                                int QuantityOfNewElements = 0;
                                foreach (KeyValuePair<int, ElementID> oneElement in TopElementsInThisColumn)
                                {
                                    PositionsOfElementsToBeChecked.Add(new(oneColumnData.Key, oneElement.Key));
                                    QuantityOfNewElements++;
                                    Action AfterAppearingOfNewElements = null;
                                    if (QuantityOfNewElements == TopElementsInThisColumn.Count)
                                    {
                                        CurrentQuantityOfReplacedTopElements++;
                                        if (QuantityOfColumnsWithFallingElements > 0 || CurrentQuantityOfReplacedTopElements != QuantityOfColumnsWithoutFallingElements)
                                        {
                                        }
                                        else
                                        {
                                            AfterAppearingOfNewElements += () => checkForNextMove?.Invoke(PositionsOfElementsToBeChecked);
                                        }
                                    }
                                    InstantiateNewElement(oneColumnData.Key, oneElement.Key, oneElement.Value, true, m_ScalingSpeedWhenDisappearingAfterMove, AfterAppearingOfNewElements);
                                }
                            }
                        };
                    }
                }
                ObjectsWhichShowPosition[oneColumnData.Key, oneElementData.Key].ThisUniformlyScaledObject.ChangeScale(-m_UsualScaleOfOneElement, m_ScalingSpeedWhenDisappearingAfterMove, AfterScaling);
            }
            int DisplacementOfCurrentElement = 0;
            Dictionary<int, int> OldAndNewRowIndexes = new();
            for (int i = MinimalRowIndex; i < m_QuantityOfElementsInOneColumn; i++)
            {
                if (oneColumnData.Value.ContainsKey(i))
                {
                    DisplacementOfCurrentElement += 1;
                }
                else
                {
                    OldAndNewRowIndexes.Add(i, i - DisplacementOfCurrentElement);
                    bool IsThisTheHighestElement = true;
                    for(int j = i + 1; j < m_QuantityOfElementsInOneColumn; j++)
                    {
                        if(!oneColumnData.Value.ContainsKey(j))
                        {
                            IsThisTheHighestElement = false;
                        }
                    }
                    if (IsThisTheHighestElement)
                    {
                        AfterFalling += () =>
                        {
                            CurrentQuantityOfColumnsWithFallenElements++;
                            for (int j = MinimalRowIndex; j < m_QuantityOfElementsInOneColumn; j++)
                            {
                                if (OldAndNewRowIndexes.ContainsKey(j))
                                {
                                    (ObjectsWhichShowPosition[oneColumnData.Key, OldAndNewRowIndexes[j]], ObjectsWhichShowPosition[oneColumnData.Key, j]) = (ObjectsWhichShowPosition[oneColumnData.Key, j], ObjectsWhichShowPosition[oneColumnData.Key, OldAndNewRowIndexes[j]]);
                                }
                            }
                            for (int j = 0; j < oneColumnData.Value.Count; j++)
                            {
                                Action AfterScaling = null;
                                if (oneColumnData.Value[j].Key == MaximalRowIndex)
                                {
                                    for (int l = MinimalRowIndex; l < m_QuantityOfElementsInOneColumn; l++)
                                    {
                                        PositionsOfElementsToBeChecked.Add(new(oneColumnData.Key, l));
                                    }
                                    if (CurrentQuantityOfColumnsWithFallenElements == QuantityOfColumnsWithFallingElements)
                                    {
                                        AfterScaling += () => checkForNextMove?.Invoke(PositionsOfElementsToBeChecked);
                                    }
                                }
                                int RowIndexOfNewElement = m_QuantityOfElementsInOneColumn - (oneColumnData.Value.Count - j);
                                InstantiateNewElement(oneColumnData.Key, RowIndexOfNewElement, oneColumnData.Value[j].Value, true, m_ScalingSpeedWhenDisappearingAfterMove, AfterScaling);
                            }
                        };
                    }
                    PlaceOneElementToPlaceOfAnother(oneColumnData.Key, i, oneColumnData.Key, i - DisplacementOfCurrentElement, m_FallingSpeedInPixels, AfterFalling);
                    AfterFalling = null;
                }
            }
        }
    }
}