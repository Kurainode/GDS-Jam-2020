using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    public Slider slider;
    public Text value;
    public TextSpeedEvent textSpeedChanged;

    private void Start()
    {
        slider.value = 20;
        slider.onValueChanged.AddListener((x) => value.text = x.ToString("F1"));
        slider.onValueChanged.AddListener((x) => textSpeedChanged.Invoke(x));
        value.text = slider.value.ToString("F1");
    }
}

[Serializable]
public class TextSpeedEvent : UnityEvent<float> { }