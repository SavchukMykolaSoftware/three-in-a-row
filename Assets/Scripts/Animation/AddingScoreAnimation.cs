using UnityEngine;
using UnityEngine.UI;

public class AddingScoreAnimation : MonoBehaviour
{
    [SerializeField] private UniformlyMovingUIObject m_ObjectToBeMoved;
    [SerializeField] private Text m_TextWithQuantityOfPointsToBeAdded;
    [SerializeField] private string m_PrefixForTextWithQuantityOfPoints = "+";
    [SerializeField] private float m_SpeedOfMoving = 5;

    private Camera MainCamera;

    private void Awake()
    {
        MainCamera = Camera.main;
    }

    public void ShowAnimation(Vector2 targetPositionFromLeftBottomAngle, int quantityOfNewPoints, ScoreHandler scoreHandler = null)
    {
        if(m_TextWithQuantityOfPointsToBeAdded != null)
        {
            m_TextWithQuantityOfPointsToBeAdded.text = $"{m_PrefixForTextWithQuantityOfPoints}{quantityOfNewPoints}";
        }
        RectTransform RectTransformOfObjectToBeMoved = m_ObjectToBeMoved.GetComponent<RectTransform>();
        Vector3 CurrentLocalScale = RectTransformOfObjectToBeMoved.localScale;
        RectTransformOfObjectToBeMoved.parent = RectTransformOfObjectToBeMoved.root;
        Vector2 CurrentLocalPosition = RectTransformUtility.WorldToScreenPoint(MainCamera, RectTransformOfObjectToBeMoved.position);
        RectTransformOfObjectToBeMoved.anchorMin = Vector2.zero;
        RectTransformOfObjectToBeMoved.anchorMax = Vector2.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(RectTransformOfObjectToBeMoved, CurrentLocalPosition, MainCamera, out Vector3 WorldPosition);
        RectTransformOfObjectToBeMoved.position = WorldPosition;
        RectTransformOfObjectToBeMoved.localScale = CurrentLocalScale;
        m_ObjectToBeMoved.Move(targetPositionFromLeftBottomAngle - RectTransformOfObjectToBeMoved.anchoredPosition, m_SpeedOfMoving, () =>
        {
            if(scoreHandler != null)
            {
                scoreHandler.IncreaseScore(quantityOfNewPoints);
            }
            Destroy(m_ObjectToBeMoved.gameObject);
        });
    }
}