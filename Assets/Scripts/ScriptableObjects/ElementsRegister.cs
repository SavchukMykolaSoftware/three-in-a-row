using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementsRegister", menuName = "ScriptableObjects/ElementsRegister")]
public class ElementsRegister : ScriptableObject
{
    [SerializeField] private List<PossibleElementData> m_AllElements = new();
    public List<PossibleElementData> AllElements { get => m_AllElements; }

    public Dictionary<ElementID, PossibleElementData> GetElementsWithTheirID()
    {
        Dictionary<ElementID, PossibleElementData> Result = new();
        foreach (PossibleElementData element in AllElements)
        {
            Result.TryAdd(element.ID, element);
        }
        return Result;
    }
}