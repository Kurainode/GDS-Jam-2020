using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerColor : MonoBehaviour
{
    public Color correctColor;
    public Color wrongColor;

    public Toggle toggle;

    public void Start()
    {
        toggle.onValueChanged.AddListener((b) => SetColor(b));
    }

    public void SetColor(bool correct)
    {
        if (correct)
        {
            GetComponent<Image>().color = correctColor;
        }
        else
        {
            GetComponent<Image>().color = wrongColor;
        }
    }
}
