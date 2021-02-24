using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionDelegate : Delegate
{
    [SerializeField]
    public List<CharacterPair> characters;

    private Dictionary<string, Character> m_characters;

    private Dictionary<string, List<Dropdown.OptionData>> m_dropdownActions;

    public Dropdown character;
    public Dropdown action;

    private Action m_data;
    
    void Start()
    {
        characters.ForEach(x => {
            m_characters.Add(x.Key, x.Value);
            character.options.Add(new Dropdown.OptionData(x.Key));
            m_dropdownActions.Add(x.Key, new List<Dropdown.OptionData>());
            x.Value.actions.ForEach(y => m_dropdownActions[x.Key].Add(new Dropdown.OptionData(y.name)));
        });
    }
    
    void OnEnable()
    {
        m_data = (Action)data;

        int characterIndex = character.options.FindIndex(x => m_data.character == x.text);
        if (characterIndex > -1)
        {
            character.value = characterIndex;
        }
        else
        {
            m_data.character = character.options[0].text;
            character.value = 0;
        }

        action.options = m_dropdownActions[m_data.character];

        int actionIndex = action.options.FindIndex(x => m_data.action == x.text);
        if (actionIndex > -1)
        {
            action.value = actionIndex;
        }
        else
        {
            m_data.action = action.options[0].text;
            action.value = 0;
        }

        character.onValueChanged.RemoveAllListeners();
        action.onValueChanged.RemoveAllListeners();

        character.onValueChanged.AddListener((i) => {
            m_data.character = character.options[i].text;
            action.value = 0;
            action.options = m_dropdownActions[m_data.character];
            m_data.action = action.options[0].text;
        });
        action.onValueChanged.AddListener((i) => m_data.action = action.options[i].text);
    }
}

[Serializable]
public class CharacterPair
{
    public string Key;
    public Character Value;
}
