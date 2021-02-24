using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<CharacterAction> actions;
    public Transform sprites;

    private Dictionary<string, CharacterAction> m_actions;

    private CharacterAction m_mainAction;

    void Start()
    {
        actions.ForEach(x => m_actions.Add(x.name, x));
    }

    public void Do(string actionName)
    {
        if (m_actions.ContainsKey(actionName))
        {
            CharacterAction action = m_actions[actionName];
            Do(action);
        }
    }

    public void Do(CharacterAction action)
    {
        //Play sound

        SetSprite(action.sprite);

        if (action.duration == -1)
        {
            m_mainAction = action;
        }
        else
        {
            StartCoroutine(DurationCoroutine(action.duration));
        }
    }

    void SetSprite(string spriteName)
    {
        int childCount = sprites.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (sprites.transform.GetChild(i).name == spriteName)
                sprites.transform.GetChild(i).gameObject.SetActive(true);
            else
                sprites.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator DurationCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Do(m_mainAction);
    }
}

[Serializable]
public class CharacterAction
{
    public string name;
    public string sound;
    public float duration;
    public string sprite;
}