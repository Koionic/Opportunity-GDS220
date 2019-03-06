using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string mainMenuSceneName, gameSceneName;

    public static SceneController instance = null;

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

    void Update()
    {

    }

    public void GameScene()
    {
        LoadScene(gameSceneName, false);
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


}
