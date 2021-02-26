using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<CharacterAction> actions;
    public Transform sprites;
    public string state = "";

    public List<ActionPrefix> prefixes;
    private Dictionary<string, string> m_prefixes = new Dictionary<string, string>();

    private Dictionary<string, CharacterAction> m_actions = new Dictionary<string, CharacterAction>();

    private CharacterAction m_mainAction;

    void Start()
    {
        prefixes.ForEach(prefix => m_prefixes.Add(prefix.state, prefix.prefix));

        actions.ForEach(x => {
            m_actions.Add(x.name, x);
        });
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
        if (action.sound != "" && action.sound != null)
            SoundManager.PlaySound(action.sound);

        if (action.sprite != null && action.sprite != "")
            SetSprite(action.sprite);

        if (action.duration <= 0)
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

        if (m_prefixes.ContainsKey(state) && sprites.transform.Find(m_prefixes[state] + spriteName))
            spriteName = m_prefixes[state] + spriteName;

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

    public bool IsBlocking(string action)
    {
        if (m_actions.ContainsKey(action))
            return m_actions[action].blocking;
        return false;
    }
}

[Serializable]
public class CharacterAction
{
    public string name;
    public string sound;
    public float duration;
    public string sprite;
    public bool blocking;
}

[Serializable]
public class ActionPrefix
{
    public string state;
    public string prefix;
}