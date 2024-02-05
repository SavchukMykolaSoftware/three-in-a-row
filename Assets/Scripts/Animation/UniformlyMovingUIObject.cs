using UnityEngine;
using System;

public class UniformlyMovingUIObject : MonoBehaviour
{
    private RectTransform m_ThisRectTransform;
    private float m_Speed;
    private Vector2 m_Displacement;
    private Vector2 m_Destination;
    private float m_QuantityOfTheIterationsOnThePath;
    private Vector2 m_SegmentOfTheWayForOneIteration;
    private Vector2 m_OvercomeDistance;
    private bool m_IsMoving;
    private Action m_AfterFinishingMoving;

    private void Awake()
    {
        m_ThisRectTransform = GetComponent<RectTransform>();
    }

    public void Move(Vector2 displacement, float speed, Action afterFinishingMoving = null)
    {
        m_Speed = speed;
        m_Displacement = displacement;
        m_Destination = m_ThisRectTransform.anchoredPosition + displacement;
        m_QuantityOfTheIterationsOnThePath = m_Displacement.magnitude / m_Speed;
        m_SegmentOfTheWayForOneIteration = m_Displacement / m_QuantityOfTheIterationsOnThePath;
        m_OvercomeDistance = Vector2.zero;
        m_AfterFinishingMoving = afterFinishingMoving;
        m_IsMoving = true;
    }

    private void Update()
    {
        if (m_IsMoving)
        {
            m_ThisRectTransform.anchoredPosition += m_SegmentOfTheWayForOneIteration * Time.deltaTime;
            m_OvercomeDistance += m_SegmentOfTheWayForOneIteration * Time.deltaTime;
            if (m_OvercomeDistance.magnitude >= m_Displacement.magnitude)
            {
                m_ThisRectTransform.anchoredPosition = m_Destination;
                m_IsMoving = false;
                m_AfterFinishingMoving?.Invoke();
            }
        }
    }
}