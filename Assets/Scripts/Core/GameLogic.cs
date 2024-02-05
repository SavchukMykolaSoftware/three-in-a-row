using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private int m_Length = 25;
    [SerializeField] private int m_Height = 15;

    [Header("Optional")]
    [SerializeField] private ScoreHandler m_ScoreHandler;
    [SerializeField] private int m_CostOfOneDestroyedElement = 1;

    private ElementID[,] m_CurrentGamePosition;
    private int m_IndexOfRowOfHighlightedElement = -1;
    private int m_IndexOfColumnOfHighlightedElement = -1;

    public static GameLogic instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        BoardVisualizer.instance.InitializeBoardParameters(m_Length, m_Height);
        m_CurrentGamePosition = new ElementID[m_Length, m_Height];
        GetNewPosition();
        ShowStartingPosition();
    }

    private void GetNewPosition()
    {
        for (int i = 0; i < m_Length; i++)
        {
            for (int j = 0; j < m_Height; j++)
            {
                ElementID IdOfElementOnThisPosition;
                if ((i == 1 && j == 1) || (i == 0 && j == 2))
                {
                    IdOfElementOnThisPosition = m_CurrentGamePosition[0, 0];
                }
                else
                {
                    List<ElementID> ExcludedValues = new();
                    if (i >= 2 && m_CurrentGamePosition[i - 1, j] == m_CurrentGamePosition[i - 2, j])
                    {
                        ExcludedValues.Add(m_CurrentGamePosition[i - 1, j]);
                    }
                    if (j >= 2 && m_CurrentGamePosition[i, j - 1] == m_CurrentGamePosition[i, j - 2])
                    {
                        ExcludedValues.Add(m_CurrentGamePosition[i, j - 1]);
                    }
                    if (i + j == 1)
                    {
                        ExcludedValues.Add(m_CurrentGamePosition[0, 0]);
                    }
                    IdOfElementOnThisPosition = RandomEnumGenerator.RandomEnumValue(ExcludedValues);
                }
                m_CurrentGamePosition[i, j] = IdOfElementOnThisPosition;
            }
        }
    }

    private void ShowStartingPosition()
    {
        BoardVisualizer.instance.ShowPositionFromZero(m_CurrentGamePosition, false);
    }

    private bool AreElementCorrdinatesValid(Vector2Int coordinates) => AreElementCorrdinatesValid(coordinates.x, coordinates.y);
    private bool AreElementCorrdinatesValid(int columnIndex, int rowIndex)
    {
        return columnIndex >= 0 && columnIndex < m_Length && rowIndex >= 0 && rowIndex < m_Height;
    }

    private bool AreTwoElementsNeighboring(Vector2Int position1, Vector2Int position2)
    {
        return (position1 - position2).sqrMagnitude == 1;
    }

    private bool FindAllElementsAsLineOfSameElements(Vector2Int coordinatesOfElementToBeChecked, out List<Vector2Int> positionsOfAllElementsInLine)
    {
        positionsOfAllElementsInLine = new() { coordinatesOfElementToBeChecked };
        ElementID IdOfElementToBeChecked = m_CurrentGamePosition[coordinatesOfElementToBeChecked.x, coordinatesOfElementToBeChecked.y];
        Vector2Int PositionOfCurrentElement = coordinatesOfElementToBeChecked;
        List<List<Vector2Int>> AllDirections = new() { new() { Vector2Int.up, Vector2Int.down }, new() { Vector2Int.left, Vector2Int.right } };
        List<Vector2Int> FoundElementsInCurrentDirection = new();
        bool DoesLineExist = false;
        foreach (List<Vector2Int> OneDirection in AllDirections)
        {
            FoundElementsInCurrentDirection.Clear();
            foreach (Vector2Int DeltaPosition in OneDirection)
            {
                PositionOfCurrentElement += DeltaPosition;
                while (AreElementCorrdinatesValid(PositionOfCurrentElement) && m_CurrentGamePosition[PositionOfCurrentElement.x, PositionOfCurrentElement.y] == IdOfElementToBeChecked)
                {
                    FoundElementsInCurrentDirection.Add(PositionOfCurrentElement);
                    PositionOfCurrentElement += DeltaPosition;
                }
                PositionOfCurrentElement = coordinatesOfElementToBeChecked;
            }
            if (FoundElementsInCurrentDirection.Count >= 2)
            {
                positionsOfAllElementsInLine.AddRange(FoundElementsInCurrentDirection);
                DoesLineExist = true;
            }
        }
        if (!DoesLineExist)
        {
            positionsOfAllElementsInLine.Clear();
        }
        return DoesLineExist;
    }

    public void CancelCurrentSelection()
    {
        m_IndexOfColumnOfHighlightedElement = -1;
        m_IndexOfRowOfHighlightedElement = -1;
    }

    public void SelectElement(int columnIndex, int rowIndex)
    {
        if (m_IndexOfColumnOfHighlightedElement != -1)
        {
            BoardVisualizer.instance.UnhighlightElement(m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement);
        }

        if (m_IndexOfColumnOfHighlightedElement == columnIndex && m_IndexOfRowOfHighlightedElement == rowIndex)
        {
            CancelCurrentSelection();
        }
        else if (m_IndexOfColumnOfHighlightedElement == -1 || !AreTwoElementsNeighboring(new Vector2Int(columnIndex, rowIndex), new Vector2Int(m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement)))
        {
            m_IndexOfColumnOfHighlightedElement = columnIndex;
            m_IndexOfRowOfHighlightedElement = rowIndex;
            BoardVisualizer.instance.HighlightElement(m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement);
        }
        else
        {
            (m_CurrentGamePosition[columnIndex, rowIndex], m_CurrentGamePosition[m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement]) = (m_CurrentGamePosition[m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement], m_CurrentGamePosition[columnIndex, rowIndex]);
            bool IsMoveValid = FindAllElementsAsLineOfSameElements(new Vector2Int(m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement), out List<Vector2Int> foundElements1) | FindAllElementsAsLineOfSameElements(new Vector2Int(columnIndex, rowIndex), out List<Vector2Int> foundElements2);
            if (IsMoveValid && m_CurrentGamePosition[m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement] == m_CurrentGamePosition[columnIndex, rowIndex])
            {
                if (foundElements1.Count != 0)
                {
                    foundElements2.Clear();
                }
            }
            Action AfterSwapping = null;
            if (IsMoveValid)
            {
                BoardVisualizer.instance.IsShowingMoveNow = true;
                AfterSwapping += () =>
                {
                    IEnumerable<Vector2Int> AllFoundElements = foundElements2.Concat(foundElements1);
                    DestroyLine(AllFoundElements);
                };
                if (m_IndexOfRowOfHighlightedElement + 1 == rowIndex)
                {
                    rowIndex--;
                    m_IndexOfRowOfHighlightedElement++;
                }
                BoardVisualizer.instance.SwapTwoElements(columnIndex, rowIndex, m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement, AfterSwapping);
            }
            else
            {
                BoardVisualizer.instance.IsShowingMoveNow = true;
                BoardVisualizer.instance.SwapTwoElementsWithReturning(columnIndex, rowIndex, m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement, () => BoardVisualizer.instance.IsShowingMoveNow = false);
                (m_CurrentGamePosition[columnIndex, rowIndex], m_CurrentGamePosition[m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement]) = (m_CurrentGamePosition[m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement], m_CurrentGamePosition[columnIndex, rowIndex]);
            }

            CancelCurrentSelection();
        }
    }

    public void SelectElement(Vector2Int coordinates) => SelectElement(coordinates.x, coordinates.y);

    public bool MakeMoveIfPossible(List<Vector2Int> coordinatesOfFieldsToBeChecked)
    {
        bool IsMoveValid = false;
        IEnumerable<Vector2Int> AllElementsToBeDestroyed = new List<Vector2Int>();
        foreach (Vector2Int coordinatesOfOneFieldToBeChecked in coordinatesOfFieldsToBeChecked)
        {
            bool IsThisFieldInLine = FindAllElementsAsLineOfSameElements(new Vector2Int(coordinatesOfOneFieldToBeChecked.x, coordinatesOfOneFieldToBeChecked.y), out List<Vector2Int> foundElementsToBeDestroyedInThisLine);
            if (IsThisFieldInLine)
            {
                IsMoveValid = true;
                AllElementsToBeDestroyed = AllElementsToBeDestroyed.Concat(foundElementsToBeDestroyedInThisLine);
            }
        }
        if (IsMoveValid)
        {
            DestroyLine(AllElementsToBeDestroyed);
        }
        BoardVisualizer.instance.IsShowingMoveNow = IsMoveValid;
        if(IsMoveValid == false)
        {
            bool IsPositionValid = CheckIfPositionIsValid();
            if (IsPositionValid == false)
            {
                UpdatePosition();
            }
        }
        return IsMoveValid;
    }

    private void DestroyLine(IEnumerable<Vector2Int> allElementsToBeDestroyed)
    {
        HashSet<Vector2Int> AllElementsToBeDestroyedAsHashSet = allElementsToBeDestroyed.ToHashSet();
        Dictionary<int, List<int>> ElementsByColumnNumbers = new();
        foreach (Vector2Int foundElementInLine in AllElementsToBeDestroyedAsHashSet)
        {
            if (ElementsByColumnNumbers.ContainsKey(foundElementInLine.x))
            {
                ElementsByColumnNumbers[foundElementInLine.x].Add(foundElementInLine.y);
            }
            else
            {
                ElementsByColumnNumbers.Add(foundElementInLine.x, new() { foundElementInLine.y });
            }
        }

        List<KeyValuePair<int, List<KeyValuePair<int, ElementID>>>> ElementsToBeDestroyedAndReplacedInColumns = new();

        foreach (KeyValuePair<int, List<int>> coordinatesOfFoundElementInLine in ElementsByColumnNumbers)
        {
            List<KeyValuePair<int, ElementID>> rowIndexesAndReplacingElements = new();
            rowIndexesAndReplacingElements.Clear();
            foreach (int rowIndex in coordinatesOfFoundElementInLine.Value)
            {
                rowIndexesAndReplacingElements.Add(new(rowIndex, RandomEnumGenerator.RandomEnumValue<ElementID>()));
            }
            rowIndexesAndReplacingElements = rowIndexesAndReplacingElements.OrderBy(keyValuePair => keyValuePair.Key).ToList();
            int MinimalRowIndex = rowIndexesAndReplacingElements[0].Key;
            int MaximalRowIndex = rowIndexesAndReplacingElements[^1].Key;
            ElementsToBeDestroyedAndReplacedInColumns.Add(new(coordinatesOfFoundElementInLine.Key, rowIndexesAndReplacingElements));

            int DisplacementOfCurrentElement = 0;
            for (int i = MinimalRowIndex; i < m_Height; i++)
            {
                if (rowIndexesAndReplacingElements.ContainsKey(i))
                {
                    DisplacementOfCurrentElement += 1;
                }
                else
                {
                    int ColumnIndex = coordinatesOfFoundElementInLine.Key;
                    (m_CurrentGamePosition[ColumnIndex, i], m_CurrentGamePosition[ColumnIndex, i - DisplacementOfCurrentElement]) = (m_CurrentGamePosition[ColumnIndex, i - DisplacementOfCurrentElement], m_CurrentGamePosition[ColumnIndex, i]);
                }
            }
            for (int k = 0; k < rowIndexesAndReplacingElements.Count; k++)
            {
                int ColumnIndex = coordinatesOfFoundElementInLine.Key;
                m_CurrentGamePosition[ColumnIndex, m_Height - (rowIndexesAndReplacingElements.Count - k)] = rowIndexesAndReplacingElements[k].Value;
            }
        }

        BoardVisualizer.instance.DestroyAndReplaceElements(ElementsToBeDestroyedAndReplacedInColumns,
            coordinatesOfFieldsToBeChecked => MakeMoveIfPossible(coordinatesOfFieldsToBeChecked),
            (addingScoreAnimation, targetPosition) => addingScoreAnimation.ShowAnimation(targetPosition, m_CostOfOneDestroyedElement, m_ScoreHandler));
    }

    private bool CheckIfPositionIsValid()
    {
        for(int i = 0; i < m_Length; i++)
        {
            for (int j = 0; j < m_Height; j++)
            {
                ElementID IdOfCurrentElement = m_CurrentGamePosition[i, j];
                List<List<Vector2Int>> SetsWithPotentialMoves = new()
                {
                    new() { new(i + 1, j), new(i + 2, j) },
                    new() { new(i - 1, j), new(i - 2, j) },
                    new() { new(i, j + 1), new(i, j + 2) },
                    new() { new(i, j - 1), new(i, j - 2) },

                    new() { new(i + 1, j + 1), new(i + 1, j + 2) },
                    new() { new(i + 1, j + 1), new(i + 2, j + 1) },
                    new() { new(i + 1, j - 1), new(i + 1, j - 2) },
                    new() { new(i + 1, j - 1), new(i + 2, j - 1) },
                    new() { new(i - 1, j - 1), new(i - 1, j - 2) },
                    new() { new(i - 1, j - 1), new(i - 2, j - 1) },
                    new() { new(i - 1, j + 1), new(i - 1, j + 2) },
                    new() { new(i - 1, j + 1), new(i - 2, j + 1) },

                    new() { new(i + 1, j + 1), new(i + 1, j - 1) },
                    new() { new(i + 1, j + 1), new(i - 1, j - 1) },
                    new() { new(i + 1, j - 1), new(i - 1, j - 1) },
                    new() { new(i - 1, j - 1), new(i - 1, j + 1) }
                };

                foreach(List<Vector2Int> oneSetOfElements in SetsWithPotentialMoves)
                {
                    bool IfThisSetContainsPossibleMove = true;
                    foreach(Vector2Int oneElement in oneSetOfElements)
                    {
                        if(AreElementCorrdinatesValid(oneElement) == false || IdOfCurrentElement != m_CurrentGamePosition[oneElement.x, oneElement.y])
                        {
                            IfThisSetContainsPossibleMove = false;
                            break;
                        }
                    }
                    if(IfThisSetContainsPossibleMove)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool UpdatePosition()
    {
        void DestroyAndSpawnNewElements()
        {
            BoardVisualizer.instance.DestroyAllVisualElements(() =>
            {
                GetNewPosition();
                BoardVisualizer.instance.ShowPositionFromZero(m_CurrentGamePosition, true, () => BoardVisualizer.instance.IsShowingMoveNow = false);
            });
        }
        if (BoardVisualizer.instance.IsShowingMoveNow)
        {
            return false;
        }
        BoardVisualizer.instance.IsShowingMoveNow = true;
        if (m_IndexOfColumnOfHighlightedElement != -1)
        {
            BoardVisualizer.instance.UnhighlightElement(m_IndexOfColumnOfHighlightedElement, m_IndexOfRowOfHighlightedElement, DestroyAndSpawnNewElements);
            CancelCurrentSelection();
        }
        else
        {
            DestroyAndSpawnNewElements();
        }
        return true;
    }
}