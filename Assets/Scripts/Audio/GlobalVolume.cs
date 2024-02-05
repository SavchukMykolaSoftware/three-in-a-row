using UnityEngine;
using UnityEngine.UI;

public class GlobalVolume : MonoBehaviour
{
    [SerializeField] private Slider m_GlobalVolumeSlider;

    private string m_KeyForPlayerPrefs;

    private void Awake()
    {
        m_KeyForPlayerPrefs = PlayerPrefsKeys.GlobalVolumeKey;
        float CurrentGlobalVolume = PlayerPrefs.HasKey(m_KeyForPlayerPrefs) ? PlayerPrefs.GetFloat(m_KeyForPlayerPrefs) : 1;
        SetGlobalVolume(CurrentGlobalVolume);
        if (m_GlobalVolumeSlider != null)
        {
            m_GlobalVolumeSlider.minValue = 0;
            m_GlobalVolumeSlider.maxValue = 1;
            m_GlobalVolumeSlider.value = CurrentGlobalVolume;
            m_GlobalVolumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        }
    }

    private void SetGlobalVolume(float newGlobalVolume)
    {
        AudioListener.volume = newGlobalVolume;
        PlayerPrefs.SetFloat(m_KeyForPlayerPrefs, newGlobalVolume);
    }
}