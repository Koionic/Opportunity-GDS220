using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const string mainMenuSceneName = "Main Menu", gameSceneName = "Tester";

    public static SceneController instance = null;

    public enum CurrentScene { MainMenu , Game };

    CurrentScene currentScene = CurrentScene.MainMenu;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        
        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {

    }

    public void UpdateSceneState()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case (mainMenuSceneName):
                {
                    currentScene = CurrentScene.MainMenu;
                }
                break;

            case (gameSceneName):
                {
                    currentScene = CurrentScene.Game;
                }
                break;

            default:
                {
                    currentScene = CurrentScene.MainMenu;
                }
                break;


        }
    }

    public void GameScene()
    {
        LoadScene(gameSceneName, true);
    }

    public void MainMenu()
    {
        LoadScene(mainMenuSceneName, true);
    }

    void LoadScene(string scene, bool instant)
    {
        LoadingController.instance.StartLoadingScreen(scene, instant);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    public bool CurrentSceneIs(CurrentScene scene)
    {
        return currentScene == scene;
    }

}
