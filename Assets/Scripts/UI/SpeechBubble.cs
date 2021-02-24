using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public Text content;
    public string text;
    public float textSpeed;

    private float m_carret;
    private AudioSource m_audioSource;
    private float m_deltaTime;

    void Start()
    {
        m_carret = 0;
        content.text = text;
        m_audioSource = GetComponent<AudioSource>();
        StartCoroutine("TextCoroutine");
    }

    void Update()
    {
        m_deltaTime = Time.deltaTime;
    }

    IEnumerator TextCoroutine()
    {
        while (m_carret < text.Length)
        {
            int old = (int)m_carret;
            m_carret += m_deltaTime * textSpeed;
            content.text = text.Substring(0, (int)m_carret);
            if (old < (int)m_carret)
            {
                if (old % 2 == 0)
                    m_audioSource.Play();
                char lastChar = content.text[(int)m_carret - 1];
                if (lastChar == '.' || lastChar == '!' || lastChar == '?')
                    yield return new WaitForSeconds(0.5f);
                if (lastChar == ',' || lastChar == ';' || lastChar == ':')
                    yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }
    }
}
