using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float soundVolume = 1.0f;

    public Transform musics;
    public Transform sounds;

    public VolumeSlider masterVolumeSlider;

    public static SoundManager instance;

    private Dictionary<string, AudioSource> m_musics = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> m_sounds = new Dictionary<string, AudioSource>();

    private AudioSource m_currentMusic = null;

    private void Awake()
    {
        if (instance)
            Destroy(instance);

        instance = this;
    }

    void Start()
    {
        int count = musics.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform child = musics.GetChild(i);
            m_musics.Add(child.name, child.GetComponent<AudioSource>());
        }
        count = sounds.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform child = sounds.GetChild(i);
            m_sounds.Add(child.name, child.GetComponent<AudioSource>());
        }

        masterVolumeSlider.volumeChanged.AddListener(x =>
        {
            m_currentMusic.volume = x * musicVolume;
            masterVolume = x;
        });
    }

    public static void PlayMusic(string name)
    {
        instance.InstancePlayMusic(name);
    }

    void InstancePlayMusic(string name)
    {
        if (m_currentMusic != null)
        {
            m_currentMusic.Stop();
            m_currentMusic.gameObject.SetActive(false);
        }
        if (m_musics.ContainsKey(name))
        {
            m_currentMusic = m_musics[name];
            m_currentMusic.volume = masterVolume * musicVolume;
            m_currentMusic.gameObject.SetActive(true);
            m_currentMusic.Play();
        }
    }

    public static void PlaySound(string name)
    {
        instance.InstancePlaySound(name);
    }

    void InstancePlaySound(string name)
    {
        if (m_sounds.ContainsKey(name))
        {
            m_sounds[name].PlayOneShot(m_sounds[name].clip, masterVolume * soundVolume * m_sounds[name].volume);
        }
    }

    public static void StopSound(string name)
    {
        instance.InstanceStopSound(name);
    }

    void InstanceStopSound(string name)
    {
        if (m_sounds.ContainsKey(name))
        {
            m_sounds[name].Stop();
        }
    }
}
