using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] 
    GameObject loadingScreen;
    [SerializeField] 
    TextMeshProUGUI loadingPercentText;
    [SerializeField] 
    TextMeshProUGUI loadingText;

    [SerializeField] 
    GameObject pauseScreen;

    [SerializeField] 
    GameObject gameHUD;
    [SerializeField] 
    Image batteryLife;

    [SerializeField] 
    GameObject cameraHUD;
    [SerializeField]
    Slider zoomSlider;
    [SerializeField]
    TextMeshProUGUI targetText;
    [SerializeField]
    RawImage newPhotoUI;

    [SerializeField] 
    GameObject gameOverScreen;

    public static UIController instance = null;

    RoverController roverController;

    PhotoCamera roverCamera;

    RoverStats roverStats;

    bool inGame, inMainMenu, inPauseMenu, inGameOver, inCameraMode;

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

        if (inGame)
        {
            if (roverController != null)
            {
                roverStats = roverController.stats;
            }
            else if (RoverController.instance != null)
            {
                roverController = RoverController.instance;
                roverCamera = roverController.gameObject.GetComponentInChildren<PhotoCamera>();
            }
        }

        UpdateCursor();

        UpdateLoadingScreen();

        UpdatePauseScreen();

        UpdateGameHUD();

        UpdateCameraHUD();

        UpdateGameOverScreen();
    }

    void UpdateLoadingScreen()
    {
        if (LoadingController.instance.HasLoaded())
        {
            if (LoadingController.instance.IsReady())
            {
                loadingPercentText.text = "100%";
                loadingPercentText.color = Color.green;

                loadingText.text = "Press Space To Start";
            }
            else
            {
                loadingPercentText.text = LoadingController.instance.LoadProgress().ToString("F") + "%";
                loadingPercentText.color = Color.white;
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
        if (inGame)
        {
            if (inPauseMenu)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    GameController.instance.Resume();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    GameController.instance.Pause();
                }
            }

            pauseScreen.SetActive(inPauseMenu);
        }
    }

    void UpdateGameHUD()
    {
        if (inGame)
        {
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
        gameHUD.SetActive(inGame);
    }

    void UpdateCameraHUD()
    {
        if (inGame)
        {
            zoomSlider.value = roverCamera.zoomPercent;

            string targetStatus = "";

            if (QuestController.instance.currentQuestType == QuestType.Photo)
            {
                if (roverCamera.targetInView && roverCamera.targetInRange && !roverCamera.targetObscured)
                {
                    targetStatus = "Target in View";
                }
                else
                {
                    if (!roverCamera.targetInView)
                    {
                        targetStatus = "Target not in view";
                    }
                    else if (!roverCamera.targetInRange)
                    {
                        targetStatus = "Target too far away";
                    }
                    else if (roverCamera.targetObscured)
                    {
                        targetStatus = "Target is obscured";
                    }
                }
            }

            targetText.text = targetStatus;

            cameraHUD.SetActive(roverController.cameraMode);
        }
    }

    public void ShowNewPhoto(Texture2D newPhotoTexture)
    {
        newPhotoUI.gameObject.SetActive(true);
        newPhotoUI.texture = newPhotoTexture;
    }

    public void DeleteNewPhoto()
    {
        newPhotoUI.texture = null;
        newPhotoUI.gameObject.SetActive(false); 
    }

    void UpdateGameOverScreen()
    {
        gameOverScreen.SetActive(GameController.instance.GameOver());
    }

    void CheckGameState()
    {
        if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.Game))
        {
            inGameOver = GameController.instance.GameOver();
            inGame = !inGameOver;
            inMainMenu = false;
            inPauseMenu = GameController.instance.IsPaused() && inGame;
            if (roverController != null)
            {
                inCameraMode = roverController.cameraMode;
            }
        }
        else if (SceneController.instance.CurrentSceneIs(SceneController.CurrentScene.MainMenu))
        {
            inGame = false;
            inMainMenu = true;
            inPauseMenu = false;
            inGameOver = false;
            inCameraMode = false;
        }
    }

    void UpdateCursor()
    {
        if (inGame)
        {
            if (inPauseMenu || inGameOver)
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
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

}
