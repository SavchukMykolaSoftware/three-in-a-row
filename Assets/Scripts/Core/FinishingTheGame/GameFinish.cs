using UnityEngine;
using UnityEngine.UI;

public class GameFinish : MonoBehaviour
{
    [SerializeField] private GameObject m_FinishGameScreen;

    [Header("Showing score")]
    [SerializeField] private ScoreHandler m_ScoreHandler;
    [SerializeField] private Text m_TextWithCurrentScore;
    [SerializeField] private string m_PrefixForTextWithCurrentScore;
    [SerializeField] private string m_PostfixForTextWithCurrentScore;
    [SerializeField] private Text m_TextWithBestScore;
    [SerializeField] private string m_PrefixForTextWithBestScore;
    [SerializeField] private string m_PostfixForTextWithBestScore;
    [SerializeField] private GameObject m_MessageAboutNewBestScore;

    private void Awake()
    {
        GameEvents.OnFinishGame += FinishGame;
    }

    private void OnDestroy()
    {
        GameEvents.OnFinishGame -= FinishGame;
    }

    private void FinishGame()
    {
        Time.timeScale = 0;
        m_FinishGameScreen.SetActive(true);
        if(m_ScoreHandler != null)
        {
            ShowCurrentScore();
            ShowBestScore();
        }
    }

    private void ShowCurrentScore()
    {
        if(m_TextWithCurrentScore != null)
        {
            m_TextWithCurrentScore.text = $"{m_PrefixForTextWithCurrentScore}{m_ScoreHandler.CurrentScore}{m_PostfixForTextWithCurrentScore}";
        }
    }

    private void ShowBestScore()
    {
        int CurrentBestScore;
        if (m_ScoreHandler.CurrentScore > m_ScoreHandler.PreviousBestScore)
        {
            CurrentBestScore = m_ScoreHandler.CurrentScore;
            if (m_MessageAboutNewBestScore != null)
            {
                m_MessageAboutNewBestScore.SetActive(true);
            }
        }
        else
        {
            CurrentBestScore = m_ScoreHandler.PreviousBestScore;
        }
        if (m_TextWithCurrentScore != null)
        {
            m_TextWithBestScore.text = $"{m_PrefixForTextWithBestScore}{CurrentBestScore}{m_PostfixForTextWithBestScore}";
        }
    }
}