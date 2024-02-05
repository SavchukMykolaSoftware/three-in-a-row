using UnityEngine;

[System.Serializable]
public class PossibleElementData
{
    [SerializeField] private ElementID m_ID;
    public ElementID ID { get => m_ID; }
    
    [SerializeField] private Sprite m_Sprite;
    public Sprite Sprite { get => m_Sprite; }
    
    [SerializeField] private Color m_Color;
    public Color Color { get => m_Color; }

}