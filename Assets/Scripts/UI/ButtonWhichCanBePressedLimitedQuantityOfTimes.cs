using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonWhichCanBePressedLimitedQuantityOfTimes : MonoBehaviour
{
    [SerializeField] private int m_QuantityOfTimes = 1;
    [SerializeField] private Text m_TextWithQuantityOfTimes;

    private Button m_ThisButton;

    public event Func<bool> OnClick;

    protected void Awake()
    {
        ShowQuantityOfTimes();
        m_ThisButton = GetComponent<Button>();
        m_ThisButton.onClick.AddListener(() =>
        {
            if (m_QuantityOfTimes > 0 && OnClick != null && OnClick.Invoke())
            {
                m_QuantityOfTimes--;
                ShowQuantityOfTimes();
            }
        });
    }

    private void ShowQuantityOfTimes()
    {
        if (m_TextWithQuantityOfTimes != null)
        {
            m_TextWithQuantityOfTimes.text = m_QuantityOfTimes.ToString();
        }
    }
}