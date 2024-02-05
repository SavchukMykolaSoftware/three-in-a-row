using UnityEngine;
using System;

public class UniformlyScaledObject : MonoBehaviour
{
    private float m_Speed;
    private Vector3 m_DeltaScale;
    private Vector3 m_TargetScale;
    private float m_QuantityOfTheIterationsOnThePath;
    private Vector3 m_DeltaScaleForOneIteration;
    private Vector3 m_ScaleWhichWasAlreadyChanged;
    private bool m_IsScaling;
    private Action m_AfterFinishingScaling;

    public void ChangeScale(Vector3 deltaScale, float speed, Action afterFinishingScaling = null)
    {
        m_Speed = speed;
        m_DeltaScale = deltaScale;
        m_TargetScale = transform.localScale + deltaScale;
        m_QuantityOfTheIterationsOnThePath = (m_DeltaScale.magnitude / m_Speed);
        m_DeltaScaleForOneIteration = m_DeltaScale / m_QuantityOfTheIterationsOnThePath;
        m_ScaleWhichWasAlreadyChanged = Vector3.zero;
        m_AfterFinishingScaling = afterFinishingScaling;
        m_IsScaling = true;
    }

    private void Update()
    {
        if (m_IsScaling)
        {
            transform.localScale += m_DeltaScaleForOneIteration * Time.deltaTime;
            m_ScaleWhichWasAlreadyChanged += m_DeltaScaleForOneIteration * Time.deltaTime;
            if (m_ScaleWhichWasAlreadyChanged.magnitude >= m_DeltaScale.magnitude)
            {
                transform.localScale = m_TargetScale;
                m_IsScaling = false;
                m_AfterFinishingScaling?.Invoke();
            }
        }
    }
}