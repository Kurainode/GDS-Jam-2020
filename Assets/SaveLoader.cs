using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoader : MonoBehaviour
{
    public GameManager gameManager;

    public Button loadButton;

    public Text selected;

    public DynamicScrollView scrollView;

    private void Start()
    {
        loadButton.interactable = false;
        scrollView.onClickFileEvent.AddListener((s) => {
            selected.text = s;
            loadButton.interactable = (s.Length > 0);
        });

        loadButton.onClick.AddListener(() => LoadSave());
    }

    public void LoadSave()
    {
        try
        {
            gameManager.Load(selected.text);
        }
        catch (AssetSaveException)
        {
            Debug.Log("OOF");
        }
    }
}
