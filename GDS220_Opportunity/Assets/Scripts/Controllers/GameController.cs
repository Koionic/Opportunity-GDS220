using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    bool isPaused = false;

    public static GameController instance = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Exit()
    {
        SceneController.instance.MainMenu();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
