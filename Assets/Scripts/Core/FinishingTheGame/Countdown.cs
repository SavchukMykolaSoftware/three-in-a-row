using System;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] private float m_StartValueInSeconds = 60;

    [Header("Visual")]
    [SerializeField] private Text m_TextWithCurrentValue;

    private float m_CurrentValueInSeconds;
    private bool m_IsCountdownRunning = false;

    public event Action AfterFinishingCountdown;

    public void StartCountdown()
    {
        m_CurrentValueInSeconds = m_StartValueInSeconds;
        ShowCurrentValue();
        m_IsCountdownRunning = true;
    }

    private void FixedUpdate()
    {
        if(m_IsCountdownRunning)
        {
            m_CurrentValueInSeconds -= Time.fixedDeltaTime;
            ShowCurrentValue();
            if (m_CurrentValueInSeconds <= 0)
            {
                AfterFinishingCountdown?.Invoke();
                m_IsCountdownRunning = false;
            }
        }
    }

    private void ShowCurrentValue()
    {
        if (m_TextWithCurrentValue != null)
        {
            int CurrentValueAsInt = (int)m_CurrentValueInSeconds;
            m_TextWithCurrentValue.text = $"{CurrentValueAsInt / 60:00}:{CurrentValueAsInt % 60:00}";
        }
    }
}