using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;

    public Text text;
    public Image clock;

    public bool paused = false;

    private float m_timeLeft;

    void Start()
    {
        text.text = time.ToString((time >= 4f) ? "F0" : "F1");
        m_timeLeft = time;
    }


    void Update()
    {
        if (!paused)
        {
            m_timeLeft -= Time.deltaTime;
            if (m_timeLeft < 0f)
                m_timeLeft = 0f;
            text.text = m_timeLeft.ToString((m_timeLeft >= 4f) ? "F0" : "F1");
            clock.fillAmount = m_timeLeft / time;
            if (m_timeLeft < 4f)
                text.color = Color.red;
        }
    }

    public float GetTimeLeft()
    {
        return m_timeLeft;
    }
}
