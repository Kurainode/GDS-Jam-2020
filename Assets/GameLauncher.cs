using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    public OptionsFile options = new OptionsFile();

    public VolumeSlider volumeSlider;
    public TextSlider textSlider;
    public Dropdown resolution;

    public DynamicScrollView savePicker;
    public InputField playerNameField;
    public Text saveName;

    public string playerName = "";
    public string saveFile = "";
    public string scenario = "";

    public Button playButton;
    public Button loadButton;

    private List<Vector2Int> m_resolutions = new List<Vector2Int>();

    void Start()
    {
        DontDestroyOnLoad(this);

        m_resolutions.Add(new Vector2Int(1920, 1080));
        m_resolutions.Add(new Vector2Int(1366, 768));
        m_resolutions.Add(new Vector2Int(2560, 1440));
        m_resolutions.Add(new Vector2Int(3840, 2160));

        try
        {
            OptionsFile optionsFile = ScenarioLoader.LoadAsset<OptionsFile>("options");
            options.resolutionIndex = optionsFile.resolutionIndex;
            options.textSpeed = optionsFile.textSpeed;
            options.soundVolume = optionsFile.soundVolume;
        }
        catch
        {
        }

        playButton.interactable = false;
        loadButton.interactable = false;

        savePicker.onClickFileEvent.AddListener(s => {
            saveName.text = s;
            saveFile = s;
            loadButton.interactable = true;
        });

        playerNameField.onValueChanged.AddListener(s => {
            playerName = s;
            playButton.interactable = (s.Length > 2);
        });

        ResetOptions();
    }

    public void SaveOptions()
    {
        options.soundVolume = volumeSlider.slider.value;
        options.textSpeed = textSlider.slider.value;
        options.resolutionIndex = resolution.value;
        Vector2Int pickedResolution = m_resolutions[options.resolutionIndex];

        Screen.SetResolution(pickedResolution.x, pickedResolution.y, false);

        ScenarioSaver.SaveAsset(options, "options");
    }

    public static void SaveOptions(float soundVolume, float textSpeed)
    {
        OptionsFile optionsFile = ScenarioLoader.LoadAsset<OptionsFile>("options");

        optionsFile.soundVolume = soundVolume;
        optionsFile.textSpeed = textSpeed;

        ScenarioSaver.SaveAsset(optionsFile, "options");
    }

    public static OptionsFile GetOptions()
    {
        return ScenarioLoader.LoadAsset<OptionsFile>("options");
    }

    public void ResetOptions()
    {
        volumeSlider.slider.value = options.soundVolume;
        textSlider.slider.value = options.textSpeed;
        resolution.value = options.resolutionIndex;
    }

    public void NewGame()
    {
        saveFile = "";
        SceneManager.LoadScene("Game");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}
