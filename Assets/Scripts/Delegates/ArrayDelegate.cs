using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrayDelegate : Delegate
{
    public InputField position;
    public Button addElement;

    public ObjectDisplayer objectDisplayer;

    private Array m_data;

    void OnEnable()
    {
        m_data = (Array)data;

        position.text = "0";

        position.onEndEdit.AddListener(s => UpdatePosition(s));

        addElement.onClick.RemoveAllListeners();
        addElement.onClick.AddListener(() => objectDisplayer.CreateAtIndex(int.Parse(position.text)));
        addElement.onClick.AddListener(() => m_data = (Array)objectDisplayer.toRead);
    }

    void UpdatePosition(string s)
    {
        if (s == "")
        {
            s = "0";
        }
        int num = int.Parse(s);
        if (num > m_data.Length)
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
