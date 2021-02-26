using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Transform root;
    public Transform options;

    public Slider textSlider;
    public Slider soundSlider;

    public Button saveOptions;

    private OptionsFile optionsFile;

    public void Awake()
    {
        optionsFile = GameLauncher.GetOptions();
    }

    public void Start()
    {
        saveOptions.onClick.AddListener(() => {
            GameLauncher.SaveOptions(soundSlider.value, textSlider.value);
            optionsFile = GameLauncher.GetOptions();
        });
    }

    public void CancelOptions()
    {
        textSlider.value = optionsFile.textSpeed;
        soundSlider.value = optionsFile.soundVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Resume();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        root.gameObject.SetActive(true);
        options.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        CancelOptions();
        root.gameObject.SetActive(true);
        options.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
