using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaysDelegate : Delegate
{
    public InputField position;
    public Toggle atEnd;
    public DynamicScrollView scrollView;

    public ObjectDisplayer objectDisplayer;

    private Array m_data;

    void OnEnable()
    {
        m_data = (Array)data;

        position.text = "0";

        position.onEndEdit.AddListener(s => UpdatePosition(s));

        scrollView.onClickFileEvent.RemoveAllListeners();
        scrollView.onClickFileEvent.AddListener((filename) => objectDisplayer.CreateAtIndex<string>(int.Parse(position.text), filename));
        scrollView.onClickFileEvent.AddListener((filename) => m_data = (Array)objectDisplayer.toRead);
        scrollView.onClickFileEvent.AddListener((filename) => UpdatePosition());

        objectDisplayer.arrayDeletionEvent.RemoveAllListeners();
        objectDisplayer.arrayDeletionEvent.AddListener((x) => {
            m_data = (Array)x;
            UpdatePosition();
        });

        atEnd.onValueChanged.RemoveAllListeners();
        atEnd.onValueChanged.AddListener(x => UpdatePosition());

        if (atEnd.isOn)
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        string s = position.text;
        if (s == "")
        {
            s = "0";
        }
        int num = int.Parse(s);
        if (num > m_data.Length || atEnd.isOn)
        {
            num = m_data.Length;
            position.text = num.ToString();
        }
        else if (num < 0)
        {
            num = 0;
            position.text = num.ToString();
        }
    }

    void UpdatePosition(string s)
    {
        if (s == "")
        {
            s = "0";
        }
        int num = int.Parse(s);
        if (num > m_data.Length || atEnd.isOn)
        {
            num = m_data.Length;
            position.text = num.ToString();
        }
        else if (num < 0)
        {
            num = 0;
            position.text = num.ToString();
        }
    }
}
