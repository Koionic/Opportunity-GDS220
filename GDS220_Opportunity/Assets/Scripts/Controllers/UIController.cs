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
    TextMeshProUGUI questText;

    [SerializeField]
    Texture waypointGraphic;
    bool showWaypoint;
    Vector3 questWaypoint;
    float waypointThreshold;

    [SerializeField]
    RawImage compassUI;
    [SerializeField]
    float compassSensitivity;

    [SerializeField] 
    GameObject cameraHUD;
    [SerializeField]
    Slider zoomSlider;
    [SerializeField]
    TextMeshProUGUI targetText;
    [SerializeField]
    RawImage newPhotoUI;

    [SerializeField]
    TextMeshProUGUI sampleText;
    [SerializeField]
    float sampleTextDelay;

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

        UpdateQuestHUD();

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

        compassUI.uvRect = new Rect((roverController.fpsCamera.transform.eulerAngles.y / 360f) + .25f, 0f, compassUI.uvRect.width, 1);

        gameHUD.SetActive(inGame);
    }

    void UpdateQuestHUD()
    {
        string finalText = "";

        Quest quest = QuestController.instance.currentQuest;

        if (quest != null)
        {
            string targetText = quest.targetName;

            switch (quest.questData.questType)
            {
                case (QuestType.Photo):
                    {
                        finalText = "Take a photo of " + targetText;
                        break;
                    }

                case (QuestType.Repair):
                    {
                        finalText = "Find and repair " + targetText;
                        break;
                    }

                case (QuestType.Sample):
                    {
                        finalText = "Take a sample of " + targetText;
                        break;
                    }
            }

            showWaypoint = false;

            if (roverCamera != null)
            {
                if (roverCamera.CheckVisionOfTarget(quest.questLocation, 80f))
                {
                    questWaypoint = roverController.fpsCamera.WorldToScreenPoint(quest.questLocation);
                    showWaypoint = true;
                }
            }
            
        }

        questText.text = finalText;
    }

    private void OnGUI()
    {
        if (showWaypoint)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(questWaypoint.x, Screen.height - questWaypoint.y, 100, 20), waypointGraphic);
        }
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

    public void ChangeSampleText(string sampleName)
    {
        CancelInvoke("RemoveSampleText");

        sampleText.text = sampleName;

        Invoke("RemoveSampleText", sampleTextDelay);
    }

    void RemoveSampleText()
    {
        sampleText.text = "";
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
