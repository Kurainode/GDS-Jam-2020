using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WordQTE : MonoBehaviour
{
    public Text content;
    public string text;
    public float duration;
    public float fadeDuration;
    public Color correctColor;
    public Color incorrectColor;

    public UnityEvent onValidated;

    private string m_currentText = ""; 

    void Start()
    {
        SetOpacity(0f);
        StartCoroutine("DurationHandler");
    }

    void Update()
    {
        if (m_currentText.Length < text.Length)
        {
            m_currentText += Input.inputString;
            SetTextColor();
            ValidateText();
        }
    }

    public IEnumerator DurationHandler()
    {
        float currentDuration = 0f;
        bool fadingOut = false;

        if (fadeDuration > 0f)
            StartCoroutine("FadeIn");
        else
            SetOpacity(1f);

        while (currentDuration < duration)
        {
            yield return null;
            currentDuration += Time.deltaTime;
            if (!fadingOut && currentDuration >= (duration - fadeDuration))
            {
                fadingOut = true;
                if (fadeDuration > 0f)
                    StartCoroutine("FadeOut");
            }
        }
        Destroy(gameObject);
    }

    public IEnumerator FadeIn()
    {
        while (content.color.a < 1f)
        {
            Color c = content.color;
            c.a += Mathf.Min(Time.deltaTime / fadeDuration, 1f - c.a);
            content.color = c;
            SetTextColor();
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        while (content.color.a > 0.0f)
        {
            Color c = content.color;
            c.a -= Mathf.Min(Time.deltaTime / fadeDuration, c.a);
            content.color = c;
            SetTextColor();
            yield return null;
        }
    }

    public void SetOpacity(float opacity)
    {
        Color c = content.color;
        c.a = opacity;
        content.color = c;
    }

    public void SetTextColor()
    {
        if (text.ToLower().Substring(0, m_currentText.Length) == m_currentText.ToLower())
        {
            content.text  = "<color=#" + getCorrectColor()   + ">" + text.Substring(0, m_currentText.Length) + "</color>";
            content.text += "<color=#" + getIncorrectColor() + ">" + text.Substring(m_currentText.Length)    + "</color>";
        }
        else
        {
            content.text = "<color=#" + getIncorrectColor() + ">" + text + "</color>";
        }
    }

    public void ValidateText()
    {
        if (text.ToLower() == m_currentText.ToLower())
        {
            onValidated.Invoke();
        }
        else if (m_currentText.Length > text.Length || text.ToLower().Substring(0, m_currentText.Length) != m_currentText.ToLower())
        {
            m_currentText = "";
        }
    }

    private string getCorrectColor()
    {
        Color returnColor = correctColor;
        returnColor.a = content.color.a;
        return ColorUtility.ToHtmlStringRGBA(returnColor);
    }

    private string getIncorrectColor()
    {
        Color returnColor = incorrectColor;
        returnColor.a = content.color.a;
        return ColorUtility.ToHtmlStringRGBA(returnColor);
    }
}
