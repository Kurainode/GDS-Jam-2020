using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorLaunchSettings : MonoBehaviour
{
    public string folder = "days";
    public bool load = false;
    public string toLoad = "";

    public ScenarioManager scenarioManager;
    public DaysManager daysManager;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetLoad(Text text)
    {
        if (text && text.text.Length > 0)
        {
            toLoad = text.text;
            load = true;
        }
        else
        {
            toLoad = "";
            load = false;
        }
    }

    public void SetLoad()
    {
        toLoad = "";
        load = false;
    }

    public void SetFolder(string targetFolder)
    {
        folder = targetFolder;
    }

    public void SetScenario()
    {
        scenarioManager.filename = toLoad;
    }

    public void SetDay()
    {
        daysManager.filename = toLoad;
    }
}
