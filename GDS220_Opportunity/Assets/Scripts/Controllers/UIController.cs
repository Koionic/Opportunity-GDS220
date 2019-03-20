using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] TextMeshProUGUI loadingPercentText;
    [SerializeField] TextMeshProUGUI loadingText;

    [SerializeField] GameObject pauseScreen;

    [SerializeField] GameObject gameHUD;
    [SerializeField] Image batteryLife;

    public static UIController instance = null;

    RoverStats roverStats;

    bool inGame, inMainMenu, inPauseMenu;

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
        CheckGameState();

        UpdateLoadingScreen();

        UpdatePauseScreen();

        UpdateGameHUD();
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
        pauseScreen.SetActive(inGame && inPauseMenu);
    }

    void UpdateGameHUD()
    {
        if (inGame)
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

            if (FindObjectOfType<RoverController>() != null)
            {
                roverStats = FindObjectOfType<RoverController>().stats;

                float batteryPercentage = Mathf.InverseLerp(0, roverStats.maxBattery, roverStats.batteryLife);
                batteryLife.fillAmount = batteryPercentage;

                if (batteryLife.fillAmount > 0.5f)
                {
                    batteryLife.color = new Color((1f - batteryPercentage) * 2f, 1f, 0f);
                }
                else
                {
                    batteryLife.color = new Color(1f, batteryPercentage * 2f, 0f);
                }
            }

            gameHUD.SetActive(true);
        }
    }

    void CheckGameState()
    {
        if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.Game))
        {
            inGame = true;
            inMainMenu = false;
            inPauseMenu = GameController.instance.IsPaused();
        }
        else if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.MainMenu))
        {
            inGame = false;
            inMainMenu = true;
            inPauseMenu = false;
        }
    }
}
