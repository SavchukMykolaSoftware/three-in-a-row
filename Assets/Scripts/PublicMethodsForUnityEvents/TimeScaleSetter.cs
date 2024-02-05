using UnityEngine;

public class TimeScaleSetter : MonoBehaviour
{
    [SerializeField] private float m_TimeScaleToBeSetOnAwake = 1;

    private void Awake()
    {
        SetTimeScale(m_TimeScaleToBeSetOnAwake);
    }

    public void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
}