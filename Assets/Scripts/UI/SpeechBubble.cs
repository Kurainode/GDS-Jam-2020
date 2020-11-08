﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public Text content;
    public string text;
    public float textSpeed;

    private float m_carret;

    void Start()
    {
        m_carret = 0;
        content.text = text;
    }
    
    void Update()
    {
        if (m_carret < text.Length)
        {
            int old = (int)m_carret;
            m_carret += Time.deltaTime * textSpeed;
            content.text = text.Substring(0, (int)m_carret);
            if (old < (int)m_carret)
                GetComponent<TriggerOnSpeak>().Speak();
        }
    }
}
