using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkDelegate : Delegate
{
    public InputField text;
    public Dropdown bubbleType;
    public Dropdown speaker;

    private TalkData m_data;

    void OnEnable()
    {
        m_data = (TalkData)data;

        text.text = m_data.text;

        List<string> bubbleTypeOptions = new List<string>();
        bubbleType.options.ForEach(x => bubbleTypeOptions.Add(x.text));
        int bubbleTypeIndex = bubbleTypeOptions.FindIndex(x => m_data.bubbleType == x);
        if (bubbleTypeIndex > -1)
        {
            bubbleType.value = bubbleTypeIndex;
        }
        else
        {
            m_data.bubbleType = bubbleTypeOptions[0];
            bubbleType.value = 0;
        }

        List<string> speakerOptions = new List<string>();
        speaker.options.ForEach(x => speakerOptions.Add(x.text));
        int speakerIndex = speakerOptions.FindIndex(x => m_data.speaker == x);
        if (bubbleTypeIndex > -1)
        {
            speaker.value = speakerIndex;
        }
        else
        {
            m_data.speaker = speakerOptions[0];
            speaker.value = 0;
        }

        text.onValueChanged.RemoveAllListeners();
        bubbleType.onValueChanged.RemoveAllListeners();
        speaker.onValueChanged.RemoveAllListeners();

        text.onValueChanged.AddListener((s) => m_data.text = s);
        bubbleType.onValueChanged.AddListener((i) => m_data.bubbleType = bubbleType.options[i].text);
        speaker.onValueChanged.AddListener((i) => m_data.speaker = speaker.options[i].text);
    }
}
