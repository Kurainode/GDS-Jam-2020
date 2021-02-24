using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionDelegate : Delegate
{
    public InputField time;
    public List<Toggle> correctToggles;
    public List<InputField> answers;
    public InputField question;

    private QuestionData m_data;

    private void Start()
    {
        time.onEndEdit.AddListener(s => UpdateTime(s));

        for (int i = 0; i < correctToggles.Count; ++i)
        {
            int pos = i;
            correctToggles[pos].onValueChanged.AddListener((b) => {
                if (b)
                    m_data.correctAnswerIndex = pos;
            });
        }

        for (int i = 0; i < answers.Count; ++i)
        {
            int pos = i;
            answers[pos].onValueChanged.AddListener((s) => m_data.answers[pos] = s);
        }
        question.onValueChanged.AddListener((s) => m_data.question = s);
    }

    void OnEnable()
    {
        m_data = (QuestionData)data;

        time.text = m_data.time.ToString();

        if (m_data.answers.Length != 4)
        {
            m_data.answers = new string[4]{ "", "", "", "" };
        }

        for (int i = 0; i < 4; ++i)
        {
            answers[i].text = m_data.answers[i];
        }
        question.text = m_data.question;

        if (m_data.correctAnswerIndex > 3 || m_data.correctAnswerIndex < 0)
            m_data.correctAnswerIndex = 0;
        
        correctToggles[m_data.correctAnswerIndex].isOn = true;
        for (int i = 0; i < answers.Count; ++i)
        {
            answers[i].GetComponent<AnswerColor>().SetColor((i == m_data.correctAnswerIndex));
        }
    }

    void UpdateTime(string s)
    {
        if (s == "")
        {
            s = "1";
        }
        int num = int.Parse(s);
        if (num > 999)
        {
            num = 999;
            time.text = num.ToString();
        }
        else if (num < 1)
        {
            num = 1;
            time.text = num.ToString();
        }
        m_data.time = num;
    }
}
