using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System;
using System.Reflection;

public class ObjectDisplayer : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Container;

    public Text pathText;

    public List<FieldInfo> fields = new List<FieldInfo>();
    public object baseObject;

    private Stack<object> parents;
    private List<string> parentsNames;

    public object toRead;
    private string currentName;
    
    public OnClickPropertyEvent onClickPropertyEvent;
    public OnGoingUpEvent onGoingUpEvent;
    public ArrayDeletionEvent arrayDeletionEvent;

    void Start()
    {
        //baseObject = ScenarioLoader.LoadAsset<InterviewData>("test", "interviews");
        //Debug.Log(JsonUtility.ToJson(baseObject));
        Restart();
    }

    public void Restart()
    {
        parents = new Stack<object>();
        parentsNames = new List<string>();
        parentsNames.Add("root");
        toRead = baseObject;
        if (baseObject != null)
            Refresh();
    }

    void Refresh()
    {
        foreach (Transform child in Container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        pathText.text = String.Join("/", parentsNames);

        if (toRead is Array)
        {
            Array content = (Array)toRead;

            for (int i = 0; i < content.Length; i++)
            {
                object fieldValue = content.GetValue(i);
                string name = currentName;
                GameObject go = Instantiate(Prefab);
                if (toRead is string[])
                    go.GetComponentInChildren<Text>().text = (string)fieldValue;
                else
                    go.GetComponentInChildren<Text>().text = i.ToString();
                go.transform.SetParent(Container);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                int buttonIndex = i;
                Transform deleteButton = go.transform.Find("Delete");
                deleteButton.gameObject.SetActive(true);
                deleteButton.GetComponent<Button>().onClick.AddListener(() => RemoveAtIndex(buttonIndex));
                go.GetComponent<Button>().onClick.AddListener(() => onClickPropertyEvent.Invoke(fieldValue, buttonIndex.ToString(), buttonIndex));
            }
        }
        else
        {
            Type myType = toRead.GetType();
            fields = new List<FieldInfo>(myType.GetFields());

            for (int i = 0; i < fields.Count; i++)
            {
                object fieldValue = fields[i].GetValue(toRead);
                string name = fields[i].Name;
                GameObject go = Instantiate(Prefab);
                go.GetComponentInChildren<Text>().text = fields[i].Name;
                go.transform.SetParent(Container);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                int buttonIndex = i;
                go.GetComponent<Button>().onClick.AddListener(() => onClickPropertyEvent.Invoke(fieldValue, name, -1));
            }
        }
    }



    public void CreateAtIndex(int index)
    {
        if (toRead is QuestionData[])
            CreateAtIndex<QuestionData>(index);
        else if (toRead is TalkData[])
            CreateAtIndex<TalkData>(index);
        else if (toRead is QTEData[])
            CreateAtIndex<QTEData>(index);
        else if (toRead is Action[])
            CreateAtIndex<Action>(index);
        else if (toRead is StringArray[])
            CreateAtIndex<StringArray>(index);
        else if (toRead is string[])
            CreateAtIndex<string>(index);
    }

    public void CreateAtIndex<T>(int index)
    {
        if (!(toRead is T[]))
            return;

        object directParent = parents.Peek();

        T[] toReadArray = (T[])toRead;
        List<T> toReadList = new List<T>();
        T created = (T)Activator.CreateInstance(typeof(T));
        for (int i = 0; i < toReadArray.Length; ++i)
        {
            if (i == index)
                toReadList.Add(created);
            toReadList.Add(toReadArray[i]);
        }
        if (index == toReadArray.Length)
        {
            toReadList.Add(created);
        }
        dynamic newValue = toReadList.ToArray();

        Type myType = directParent.GetType();
        FieldInfo field = myType.GetField(parentsNames[parentsNames.Count - 1]);
        field.SetValue(directParent, newValue);
        toRead = newValue;
        Refresh();
    }

    public void CreateAtIndex<T>(int index, T data)
    {
        if (!(toRead is T[]))
            return;

        object directParent = parents.Peek();

        T[] toReadArray = (T[])toRead;
        List<T> toReadList = new List<T>();
        for (int i = 0; i < toReadArray.Length; ++i)
        {
            if (i == index)
                toReadList.Add(data);
            toReadList.Add(toReadArray[i]);
        }
        if (index == toReadArray.Length)
        {
            toReadList.Add(data);
        }
        dynamic newValue = toReadList.ToArray();

        Type myType = directParent.GetType();
        FieldInfo field = myType.GetField(parentsNames[parentsNames.Count - 1]);
        field.SetValue(directParent, newValue);
        toRead = newValue;
        Refresh();
    }

    public void RemoveAtIndex(int index)
    {
        if (toRead is QuestionData[])
            RemoveAtIndex<QuestionData>(index);
        else if (toRead is TalkData[])
            RemoveAtIndex<TalkData>(index);
        else if (toRead is QTEData[])
            RemoveAtIndex<QTEData>(index);
        else if (toRead is Action[])
            RemoveAtIndex<Action>(index);
        else if (toRead is StringArray[])
            RemoveAtIndex<StringArray>(index);
        else if (toRead is string[])
            RemoveAtIndex<string>(index);
    }

    public void RemoveAtIndex<T>(int index)
    {
        if (!(toRead is T[]))
            return;

        object directParent = parents.Peek();

        T[] toReadArray = (T[])toRead;
        List<T> toReadList = new List<T>();
        for (int i = 0; i < toReadArray.Length; ++i)
        {
            if (i != index)
                toReadList.Add(toReadArray[i]);
        }
        dynamic newValue = toReadList.ToArray();

        Type myType = directParent.GetType();
        FieldInfo field = myType.GetField(parentsNames[parentsNames.Count - 1]);
        field.SetValue(directParent, newValue);
        toRead = newValue;


        Refresh();
        arrayDeletionEvent.Invoke(toRead);
    }

    public void GoUp()
    {
        if (parents.Count > 0)
        {
            if (parents.Peek() is int)
            {
                parents.Pop();
            }
            toRead = parents.Pop();
            parentsNames.RemoveAt(parentsNames.Count - 1);
        }
        onGoingUpEvent.Invoke(toRead);
        Refresh();
    }

    public void GoDown(object child, string parentName, int index = -1)
    {
        parents.Push(toRead);
        if (index >= 0)
        {
            parents.Push(index);
        }
        parentsNames.Add(parentName);
        toRead = child;
        Refresh();
    }
}

[System.Serializable]
public class OnClickPropertyEvent : UnityEvent<object, string, int> { }

[System.Serializable]
public class OnGoingUpEvent : UnityEvent<object> { }

[System.Serializable]
public class ArrayDeletionEvent : UnityEvent<object> { }