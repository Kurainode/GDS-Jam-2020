using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public Text value;
    public SoundVolumeEvent volumeChanged;

    private void Start()
    {
        slider.onValueChanged.AddListener((x) => value.text = x.ToString("F2"));
        slider.onValueChanged.AddListener((x) => volumeChanged.Invoke(x));
        value.text = slider.value.ToString("F2");
    }
}

[Serializable]
public class SoundVolumeEvent : UnityEvent<float> { }