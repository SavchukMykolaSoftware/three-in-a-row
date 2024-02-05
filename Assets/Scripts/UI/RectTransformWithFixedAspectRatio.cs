using UnityEngine;

public class RectTransformWithFixedAspectRatio : MonoBehaviour
{
    [SerializeField] private RectTransform m_ThisRectTransform;
    [SerializeField] private float m_AspectRatio = 1;
    [SerializeField] private float m_LeftMargin = 37.5f;
    [SerializeField, Range(0, 1)] private float m_MinimalPercentOfBoardWhichShouldBeFreeOnRightSide = 0.4f;

    private void Awake()
    {
        float ScreenWidth = Screen.width;
        float MinimalRightMargin = m_MinimalPercentOfBoardWhichShouldBeFreeOnRightSide * ScreenWidth;
        float CurrentLength = m_ThisRectTransform.rect.height * m_AspectRatio;
        float ScaleCoefficient = Mathf.Max(1, (m_LeftMargin + CurrentLength) / (ScreenWidth - MinimalRightMargin));
        m_ThisRectTransform.sizeDelta = new Vector2(CurrentLength, m_ThisRectTransform.sizeDelta.y);
        Vector3 NewLocalScale = m_ThisRectTransform.localScale / ScaleCoefficient;
        m_ThisRectTransform.localScale = NewLocalScale;
        m_ThisRectTransform.anchoredPosition = new Vector2(NewLocalScale.x * (m_LeftMargin + CurrentLength / 2), m_ThisRectTransform.anchoredPosition.y);
    }
}