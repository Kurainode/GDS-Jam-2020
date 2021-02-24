using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject background;

    public ObjectDisplayer objectDisplayer;
    public EditorLaunchSettings settings = null;

    public FileSaver fileSaver;

    protected string saveFolder = "mornings";

    [SerializeField]
    public List<DelegateInfo> delegates;
    protected Dictionary<string, Delegate> m_delegates = new Dictionary<string, Delegate>();

    protected object m_editorData;

    protected virtual void Start()
    {
        settings = GameObject.FindObjectOfType<EditorLaunchSettings>();
        if (settings)
        {
            if (settings.folder == "mornings")
            {
                if (settings.load)
                    m_editorData = ScenarioLoader.LoadAsset<MorningData>(settings.toLoad, settings.folder);
                else
                    m_editorData = new MorningData();
                SetBackground("MorningEditor");
            }
            else if (settings.folder == "interviews")
            {
                if (settings.load)
                    m_editorData = ScenarioLoader.LoadAsset<InterviewData>(settings.toLoad, settings.folder);
                else
                    m_editorData = new InterviewData();
                SetBackground("InterviewEditor");
            }
            else
            {
                if (settings.load)
                    m_editorData = ScenarioLoader.LoadAsset<MeetingData>(settings.toLoad, settings.folder);
                else
                    m_editorData = new MeetingData();
                SetBackground("MeetingEditor");
            }
            saveFolder = settings.folder;
            fileSaver.fileName.text = settings.toLoad;
            GameObject.Destroy(settings.gameObject);
        }
        else
        {
            m_editorData = new MorningData();
            SetBackground("MorningEditor");
        }
        fileSaver.asset = m_editorData;
        fileSaver.folder = saveFolder;

        objectDisplayer.baseObject = m_editorData;
        objectDisplayer.onClickPropertyEvent.AddListener((o, s, i) => ObjectClickHandler(o, s, i));
        objectDisplayer.onGoingUpEvent.AddListener((o) => ObjectClickHandler(o));
        objectDisplayer.Restart();

        delegates.ForEach(x => m_delegates.Add(x.name, x.@delegate));
    }

    public virtual void ObjectClickHandler(object child, string parentName = null, int index = -1)
    {
        if (child is string || child is int || child is float || child is string[])
        {
            //No action
            return;
        }

        SetDelegate(null);

        if (child is QTEData)
        {
            SetDelegate(child, "QTEDelegate");
        }
        else if (child is TalkData)
        {
            SetDelegate(child, "TalkDelegate");
        }
        else if (child is QuestionData)
        {
            SetDelegate(child, "QuestionDelegate");
        }
        else if (child is Action)
        {
            SetDelegate(child, "ActionDelegate");
        }
        else if (child is Array)
        {
            SetDelegate(child, "ArrayDelegate");
        }
        if (parentName != null)
            objectDisplayer.GoDown(child, parentName, index);
    }

    public void SetDelegate(object data, string delegateName = null)
    {
        foreach (Delegate @delegate in m_delegates.Values)
        {
            @delegate.gameObject.SetActive(false);
        }
        if (delegateName == null)
        {
            return;
        }
        else
        {
            m_delegates[delegateName].data = data;
            m_delegates[delegateName].gameObject.SetActive(true);
        }
    }

    public void SetBackground(string name)
    {
        int childCount = background.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (background.transform.GetChild(i).name == name)
                background.transform.GetChild(i).gameObject.SetActive(true);
            else
                background.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

[Serializable]
public class DelegateInfo
{
    public string name;
    public Delegate @delegate;
}