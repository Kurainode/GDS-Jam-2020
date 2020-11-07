using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    public string questionText;
    public string[] answersText;
    public int correctAnswerIndex;

    public Text question;
    public Button[] answers;

    public UnityEvent<bool> answeredQuestion;

    public Timer timer;
    public float duration;

    private int[] m_answerOrder = { 0, 1, 2, 3 };
   
    void Start()
    {
        question.text = questionText;
        System.Random random = new System.Random();
        m_answerOrder = m_answerOrder.OrderBy(x => random.Next()).ToArray();

        for (int i = 0; i < 4; ++i)
        {
            answers[i].GetComponentInChildren<Text>().text = answersText[m_answerOrder[i]];
            if (m_answerOrder[i] == correctAnswerIndex)
                answers[i].onClick.AddListener(CorrectAnswer);
            else
                answers[i].onClick.AddListener(WrongAnswer);
        }

        timer.time = duration;
        timer.enabled = true;
    }

    void Update()
    {
        if (timer.GetTimeLeft() <= 0f)
            WrongAnswer();
    }

    void CorrectAnswer()
    {
        timer.paused = true;
        for (int i = 0; i < 4; ++i)
        {
            if (m_answerOrder[i] == correctAnswerIndex)
                answers[i].GetComponent<Image>().color = Color.green;
            else
                answers[i].GetComponent<Image>().color = Color.red;
            answers[i].GetComponent<Button>().enabled = false;
        }
        answeredQuestion.Invoke(true);
    }

    void WrongAnswer()
    {
        timer.paused = true;
        for (int i = 0; i < 4; ++i)
        {
            if (m_answerOrder[i] == correctAnswerIndex)
                answers[i].GetComponent<Image>().color = Color.green;
            else
                answers[i].GetComponent<Image>().color = Color.red;
            answers[i].GetComponent<Button>().enabled = false;
        }
        answeredQuestion.Invoke(false);
    }
}
