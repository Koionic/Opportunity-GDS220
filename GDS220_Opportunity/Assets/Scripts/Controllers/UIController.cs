using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VectorMaths;

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
    RawImage batteryIndicator;
    [SerializeField]
    Texture2D[] batteryLifePics;
    [SerializeField]
    TextMeshProUGUI batteryLifeText;

    [SerializeField]
    TextMeshProUGUI mainQuestText, social1QuestText, social2QuestText;

    TextMeshProUGUI questText;

    string finalText = "";

    [SerializeField]
    RawImage cameraWaypoint, sampleWaypoint, repairWaypoint;

    RawImage waypoint;

    bool showWaypoint;
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
    GameObject socialMediaPanel, groundControlPanel;

    [SerializeField]
    RawImage GCtrlSMdia;

    [SerializeField]
    Texture2D socialMediaSelected, groundControlSelected;

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
            compassUI.uvRect = new Rect((roverController.fpsCamera.transform.eulerAngles.y / 360f), 0f, 1, 1);

            UpdateCompassHUD(typeof(CameraQuest));
            UpdateCompassHUD(typeof(SampleQuest));
            UpdateCompassHUD(typeof(RepairQuest));

            float batteryPercentage = Mathf.InverseLerp(0, roverStats.maxBattery, roverStats.batteryLife);
            batteryLifeText.text = batteryPercentage.ToString("P1");

            if (batteryPercentage <= 0f)
            {
                batteryIndicator.texture = batteryLifePics[0];
            }
            else if (batteryPercentage <= 0.20f)
            {
                batteryIndicator.texture = batteryLifePics[1];
            }
            else if (batteryPercentage <= 0.40f)
            {
                batteryIndicator.texture = batteryLifePics[2];
            }
            else if (batteryPercentage <= 0.60f)
            {
                batteryIndicator.texture = batteryLifePics[3];
            }
            else if (batteryPercentage <= 0.80f)
            {
                batteryIndicator.texture = batteryLifePics[4];
            }
            else if (batteryPercentage <= 1f)
            {
                batteryIndicator.texture = batteryLifePics[5];
            }

        }

        gameHUD.SetActive(inGame);
    }

    void UpdateCompassHUD(System.Type type)
    {
        Quest quest = QuestController.instance.ActiveQuestOfType(type);

        if (quest != null)
        {
            GetQuestUI(type);

            if (waypoint != null)
            {
                float questAngle = Maths.GetSignedHorizontalAngle(roverController.fpsCamera.transform, quest.questLocation);

                waypoint.uvRect = new Rect((questAngle / 360f), 0, 1, 1);
            }
        }
        else
        {
            GetQuestUI(type);

            waypoint.gameObject.SetActive(false);
        }

    }

    public void ToggleStream()
    {
        if (groundControlPanel.activeSelf)
        {
            socialMediaPanel.SetActive(true);
            groundControlPanel.SetActive(false);

            GCtrlSMdia.texture = socialMediaSelected;
        }
        else if (socialMediaPanel.activeSelf)
        {
            groundControlPanel.SetActive(true);
            socialMediaPanel.SetActive(false);

            GCtrlSMdia.texture = groundControlSelected;
        }
    }

    public void StartQuestUI(Quest quest)
    {
        GetQuestUI(quest.GetType());

        waypoint.gameObject.SetActive(true);

        questText.text = finalText;
    }

    public void FinishQuestUI(Quest quest)
    {
        GetQuestUI(quest.GetType());

        questText.text = "";

        waypoint.gameObject.SetActive(false);
    }

    void GetQuestUI(System.Type type)
    {
        Quest quest = QuestController.instance.ActiveQuestOfType(type);

        string questTargetText = "";

        if (quest != null)
        {
            if (quest == QuestController.instance.mainQuest)
            {
                questText = mainQuestText;
            }
            if (quest == QuestController.instance.socialQuest1)
            {
                questText = social1QuestText;
            }
            if (quest == QuestController.instance.socialQuest2)
            {
                questText = social2QuestText;
            }

            questTargetText = quest.targetName;
        }

        if (type == typeof(CameraQuest))
        {
            waypoint = cameraWaypoint;
            finalText = "Take a photo of " + questTargetText;
        }
        else if (type == typeof(SampleQuest))
        {
            waypoint = sampleWaypoint;
            finalText = "Take a sample of " + questTargetText;
        }
        else if (type == typeof(RepairQuest))
        {
            waypoint = repairWaypoint;
            finalText = "Find and repair " + questTargetText;
        }
    }

    void UpdateCameraHUD()
    {
        if (inGame)
        {
            zoomSlider.value = roverCamera.zoomPercent;

            string targetStatus = "";

            if (QuestController.instance.ActiveQuestOfType(typeof(CameraQuest)) != null)
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
        else
        {
            cameraHUD.SetActive(false);
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
