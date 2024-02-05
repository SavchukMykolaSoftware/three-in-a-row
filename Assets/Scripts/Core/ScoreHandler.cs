using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Text m_TextWithScore;

    public int CurrentScore { get; private set; }
    public int PreviousBestScore { get; private set; }

    private void Awake()
    {
        if(PlayerPrefs.HasKey(PlayerPrefsKeys.BestScoreKey) == false)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.BestScoreKey, 0);
            PreviousBestScore = 0;
        }
        else
        {
            PreviousBestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.BestScoreKey);
        }
        ShowCurrentScore();
        GameEvents.OnFinishGame += SaveScoreIfItIsBest;
    }

    private void OnDestroy()
    {
        GameEvents.OnFinishGame -= SaveScoreIfItIsBest;
    }

    public void IncreaseScore(int scoreDelta)
    {
        CurrentScore += scoreDelta;
        ShowCurrentScore();
    }

    public void ShowCurrentScore()
    {
        if (m_TextWithScore != null)
        {
            m_TextWithScore.text = CurrentScore.ToString();
        }
    }

    public void SaveScoreIfItIsBest()
    {
        int PreviousBestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.BestScoreKey);
        if(CurrentScore > PreviousBestScore)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.BestScoreKey, CurrentScore);
        }
    }
}