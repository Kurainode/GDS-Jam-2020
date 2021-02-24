using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Transform root;
    public Transform options;

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
        root.gameObject.SetActive(true);
        options.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
