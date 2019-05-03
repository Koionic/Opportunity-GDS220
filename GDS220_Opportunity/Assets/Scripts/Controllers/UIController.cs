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
    RawImage cameraWaypoint, sampleWaypoint, repairWaypoint, homeWaypoint;

    RawImage waypoint;

    bool showWaypoint;
    float waypointThreshold;

    [SerializeField]
    RawImage compassUI;
    [SerializeField]
    GameObject compassRenderTexture;

    [SerializeField] 
    GameObject cameraHUD;
    [SerializeField]
    Slider zoomSlider;
    [SerializeField]
    TextMeshProUGUI targetText;
    [SerializeField]
    RawImage newPhotoUI;
    [SerializeField]
    GameObject submitPhotoText;

    [SerializeField]
    TextMeshProUGUI tempText;
    [SerializeField]
    float sampleTextDelay;

    [SerializeField]
    Image progressBar;

    [SerializeField]
    GameObject socialMediaPanel, groundControlPanel;

    [SerializeField]
    RawImage GCtrlSMdia;

    [SerializeField]
    Texture2D socialMediaSelected, groundControlSelected, groundControlNoSocialMedia;

    public bool groundControlEnabled, socialMediaEnabled;
    bool groundControlOn, socialMediaOn;

    [SerializeField] 
    GameObject gameOverScreen;

    [SerializeField]
    TextMeshProUGUI roverText, tutorialText;

    public static UIController instance = null;

    GameObject homeBase;

    RoverController roverController;

    PhotoCamera roverPhotoCamera;

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
                roverPhotoCamera = roverController.gameObject.GetComponentInChildren<PhotoCamera>();
            }

            if (homeBase == null)
            {
                homeBase = GameObject.FindWithTag("HomeBase");
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

                loadingText.text = "Press Enter To Start";
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
            if (compassRenderTexture.activeSelf)
            {
                compassUI.uvRect = new Rect((roverController.fpsCamera.transform.eulerAngles.y / 360f), 0f, 1, 1);

                if (homeWaypoint.IsActive())
                {
                    float angle = Maths.GetSignedHorizontalAngle(roverController.fpsCamera.transform, homeBase.transform.position);

                    homeWaypoint.uvRect = new Rect((angle / 360f), 0, 1, 1);
                }

                UpdateCompassHUD(typeof(CameraQuest));
                UpdateCompassHUD(typeof(SampleQuest));
                UpdateCompassHUD(typeof(RepairQuest));
            }
            print("ground control enabled " + groundControlEnabled);
            GCtrlSMdia.gameObject.SetActive(groundControlEnabled);
            print("ground control on " + groundControlOn);
            groundControlPanel.SetActive(groundControlOn);
            print("social media " + socialMediaOn);
            socialMediaPanel.SetActive(socialMediaOn);

            progressBar.gameObject.SetActive(roverController.actionInProgress);

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

    public void EnableCompassUI()
    {
        compassRenderTexture.gameObject.SetActive(true);
    }

    public void DisableCompassUI()
    {
        compassRenderTexture.gameObject.SetActive(false);
    }

    public void ToggleStream()
    {
        print("toggling");
        if (groundControlEnabled)
        {
            print("ground is enabled");
            if (groundControlOn)
            {
                print("ground is on");
                if (socialMediaEnabled)
                {
                    print("social media is on");
                
                    socialMediaOn = true;
                    groundControlOn = false;

                    GCtrlSMdia.texture = socialMediaSelected;
                }
            }
            else if (socialMediaOn)
            {
                groundControlOn = true;
                socialMediaOn = false;

                GCtrlSMdia.texture = groundControlSelected;
            }
        }
    }

    public void EnableGroundControl()
    {
        if (groundControlEnabled)
        {
            if (!socialMediaEnabled)
            {
                EnableSocialMedia();
            }
        }
        else
        {
            groundControlEnabled = true;
            groundControlOn = true;
            GCtrlSMdia.gameObject.SetActive(true);
        }
    }

    public void DisableGroundControl()
    {
        groundControlEnabled = false;
        groundControlOn = false;
    }

    public void EnableSocialMedia()
    {
        GCtrlSMdia.texture = groundControlSelected;
        socialMediaEnabled = true;
    }

    public void DisableSocialMedia()
    {
        GCtrlSMdia.texture = groundControlNoSocialMedia;
        socialMediaEnabled = false;
        socialMediaOn = false;
    }

    public void EnableHomeWaypoint()
    {
        homeWaypoint.gameObject.SetActive(true);
    }

    public void DisableHomeWaypoint()
    {
        homeWaypoint.gameObject.SetActive(false);
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
            zoomSlider.value = roverPhotoCamera.zoomPercent;

            string targetStatus = "";

            if (QuestController.instance.ActiveQuestOfType(typeof(CameraQuest)) != null)
            {
                if (roverPhotoCamera.targetInView && roverPhotoCamera.targetInRange && !roverPhotoCamera.targetObscured)
                {
                    targetStatus = "Target in View";
                }
                else
                {
                    if (!roverPhotoCamera.targetInView)
                    {
                        targetStatus = "Target not in view";
                    }
                    else if (!roverPhotoCamera.targetInRange)
                    {
                        targetStatus = "Target too far away";
                    }
                    else if (roverPhotoCamera.targetObscured)
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

        submitPhotoText.SetActive(true);
    }

    public void DeleteNewPhoto()
    {
        newPhotoUI.texture = null;
        newPhotoUI.gameObject.SetActive(false);

        submitPhotoText.SetActive(false);
    }

    public void ShowRoverLog(string roverString)
    {
        CancelInvoke("HideRoverLog");
        roverText.text = roverString;
    }

    public void HideRoverLog()
    {
        roverText.text = "";
    }

    public void ShowTutorialText(string tutorialString)
    {
        CancelInvoke("HideTutorialText");
        tutorialText.text = tutorialString;
    }

    public void HideTutorialText()
    {
        tutorialText.text = "";
    }

    public void ChangeTempText(string tempString)
    {
        CancelInvoke("RemoveTempText");

        tempText.text = tempString;

        Invoke("RemoveTempText", sampleTextDelay);
    }

    void RemoveTempText()
    {
        tempText.text = "";
    }

    void UpdateGameOverScreen()
    {
        gameOverScreen.SetActive(GameController.instance.GameOver());
    }

    public void UpdateProgressBar(float percent)
    {
        progressBar.fillAmount = percent;
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
