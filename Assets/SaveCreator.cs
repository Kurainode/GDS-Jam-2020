using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveCreator : MonoBehaviour
{
    public GameManager gameManager;

    public Button saveButton;
    public InputField fileName;

    public Text error;
    public Text success;

    public DynamicScrollView scrollView;

    private void Start()
    {
        fileName.onValueChanged.AddListener((s) => saveButton.interactable = (s.Length > 0));
        saveButton.interactable = (fileName.text.Length > 0);
        scrollView.onClickFileEvent.AddListener((s) => {
            fileName.text = s;
            saveButton.interactable = (s.Length > 0);
        });
    }

    public void SaveFile()
    {
        try
        {
            gameManager.Save(fileName.text);
            StartCoroutine(MessageCoroutine(success));
        }
        catch (AssetSaveException)
        {
            StartCoroutine(MessageCoroutine(error));
        }
    }

    public IEnumerator MessageCoroutine(Text message)
    {
        message.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        message.gameObject.SetActive(false);
    }
}
