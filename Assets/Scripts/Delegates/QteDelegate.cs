using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteDelegate : Delegate
{
    public InputField time;
    public InputField qte;

    private QTEData m_data;

    private void Start()
    {
        time.onEndEdit.AddListener(s => UpdateTime(s));
        
        qte.onValueChanged.AddListener((s) => m_data.qte = s);
    }

    void OnEnable()
    {
        m_data = (QTEData)data;

        time.text = m_data.time.ToString();
        qte.text = m_data.qte;
    }

    void UpdateTime(string s)
    {
        if (s == "")
        {
            s = "1";
        }
        int num = int.Parse(s);
        if (num > 99)
        {
            num = 99;
            time.text = num.ToString();
        }
        else if (num < 1)
        {
            num = 1;
            time.text = num.ToString();
        }
        m_data.time = num;
    }
}
