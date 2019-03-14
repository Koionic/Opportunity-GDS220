using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{

    [SerializeField] GameObject loadingScreen;
    [SerializeField] TextMeshProUGUI loadingPercentText;
    [SerializeField] TextMeshProUGUI loadingText;


    [SerializeField] GameObject pauseScreen;

    public static UIController instance = null;

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

    }

    // Update is called once per frame
    void Update()
    {
        UpdateLoadingScreen();

        UpdatePauseScreen();

        UpdateGeneralUI();
    }

    void UpdateLoadingScreen()
    {
        if (LoadingController.instance.HasLoaded())
        {
            if (LoadingController.instance.IsReady())
            {
                loadingPercentText.text = "100%";
                loadingPercentText.color = Color.green;

                loadingText.text = "";
            }
            else
            {
                loadingPercentText.text = LoadingController.instance.LoadProgress().ToString("F") + "%";
                loadingPercentText.color = Color.white;

                loadingText.text = "";
            }
        }
        else
        {
            loadingText.text = "Loading...";

            loadingPercentText.text = "";
        }


        loadingScreen.SetActive(LoadingController.instance.IsLoading());
    }

    void UpdatePauseScreen()
    {
        pauseScreen.SetActive(SceneController.instance.TheCurrentScene() == SceneController.CurrentScene.Game && GameController.instance.IsPaused());
    }

    void UpdateGeneralUI()
    {
        if (GameController.instance.IsPaused())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpdateBatteryLife()
    {

    }
}
