using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : EditorManager
{
    public string filename = "";

    protected override void Start()
    {
        if (filename != "" && filename != null)
        {
            try
            {
                m_editorData = ScenarioLoader.LoadAsset<GameFiles>(filename, "scenarios");
            }
            catch
            {
                m_editorData = new GameFiles();
            }
        }
        else
        {
            m_editorData = new GameFiles();
        }

        fileSaver.asset = m_editorData;
        fileSaver.folder = "scenarios";
        fileSaver.fileName.text = filename;

        objectDisplayer.baseObject = m_editorData;
        objectDisplayer.onClickPropertyEvent.AddListener((o, s, i) => ObjectClickHandler(o, s, i));
        objectDisplayer.onGoingUpEvent.AddListener((o) => ObjectClickHandler(o));
        objectDisplayer.Restart();

        delegates.ForEach(x => m_delegates.Add(x.name, x.@delegate));
    }

    private void OnEnable()
    {
        fileSaver.asset = m_editorData;
        fileSaver.fileName.text = filename;
        objectDisplayer.Restart();
        SetDelegate(null);
    }

    public override void ObjectClickHandler(object child, string parentName = null, int index = -1)
    {
        if (child is string || child is int || child is float)
        {
            //No action
            return;
        }

        SetDelegate(null);

        if (child is string[])
        {
            SetDelegate(child, "DaysDelegate");
        }
        else if (child is Array)
        {
            SetDelegate(child, "ArrayDelegate");
        }
        if (parentName != null)
            objectDisplayer.GoDown(child, parentName, index);
    }
}
