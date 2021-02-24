using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class DynamicScrollView : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Container;
    public string path = "";
    public List<string> files = new List<string>();
    [SerializeField]
    public OnClickFileEvent onClickFileEvent;

    void Start()
    {
        files = new List<string>(Directory.GetFiles(Application.persistentDataPath + "/GameData/" + path, "*.json"));
        for (int i = 0; i < files.Count; ++i)
        {
            files[i] = Path.GetFileNameWithoutExtension(files[i]);
        }

        for (int i = 0; i < files.Count; i++)
        {
            GameObject go = Instantiate(Prefab);
            go.GetComponentInChildren<Text>().text = files[i];
            go.transform.SetParent(Container);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            int buttonIndex = i;
            string file = files[i];
            go.GetComponent<Button>().onClick.AddListener(() => onClickFileEvent.Invoke(file));
        }
    }
}

[System.Serializable]
public class OnClickFileEvent : UnityEvent<string> { }