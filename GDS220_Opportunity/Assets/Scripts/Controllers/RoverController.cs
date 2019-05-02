using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoverController : MonoBehaviour
{
    WheelDrive wheelDrive;

    [SerializeField]
    float sensitivityX = 15f, sensitivityY = 15f;

    [SerializeField]
    float minimumX = -80f, maximumX = 80f;

    [SerializeField]
    float minimumY = -60f, maximumY = 90f;

    float rotationX;
    float rotationY;

    public Camera fpsCamera;

    PhotoCamera photoCamera;

    Rigidbody rb;

    public UnityEvent<Texture2D, bool> photoTaken;

    public bool cameraMode;

    bool potentialPhoto;
    bool potentialPhotoCorrect;

    public UnityEvent OutOfBattery;

    public bool compassActive = false;
    public bool groundControlActive = false;

    public bool freezeMovement = true;
    public bool freezeInPlace = true;
    public bool freezeBattery = true;

    bool outOfBattery = false;

    [SerializeField]
    float batteryDepleteRate = .5f;

    [SerializeField]
    Transform sampleOrigin;
    [SerializeField]
    float sampleDistance;

    SampleData sampleData;
    RepairData repairData;

    public RoverStats stats;

    public static RoverController instance;

    float actionProgress = 0f;
    public bool actionInProgress = false;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {
        wheelDrive = GetComponent<WheelDrive>();
        rb = GetComponent<Rigidbody>();

        photoCamera = GetComponentInChildren<PhotoCamera>();

        fpsCamera.enabled = false;
        fpsCamera.enabled = true;

        if (LevelDataHolder.instance != null)
            stats.savedLevel = LevelDataHolder.instance.currentLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeInPlace)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        if (!freezeBattery)
        {
            DepleteBattery();
        }


        if (!freezeMovement)
        {
            ProcessInputs();
        }

    }


    void DepleteBattery()
    {
        stats.batteryLife -= Time.deltaTime * batteryDepleteRate;

        outOfBattery = stats.batteryLife <= 0f;

        if (outOfBattery)
        {
            stats.batteryLife = 0f;

            FreezeRoverStates(FreezeType.Movement, true);
            FreezeRoverStates(FreezeType.Battery, true);

            OutOfBattery.Invoke();
        }
    }

    void ChargeBattery(float charge)
    {
        stats.batteryLife = Mathf.Clamp((stats.batteryLife + charge), 0f, stats.maxBattery);
    }

    void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cameraMode)
            {
                ToggleCameraMode(false);
            }
            else
            {
                ToggleCameraMode(true);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (cameraMode)
            {
                photoCamera.TriggerPhoto(Screen.width, Screen.height);
            }
            else
            {
                Ray sampleRay = new Ray(sampleOrigin.transform.position, sampleOrigin.transform.forward);
                RaycastHit raycastHit;

                if (Physics.Raycast(sampleRay, out raycastHit, sampleDistance))
                {
                    sampleData = raycastHit.collider.GetComponent<SampleData>();
                    if (sampleData != null)
                    {
                        StartAction(RoverAction.Sample);
                    }

                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!cameraMode)
            {
                Ray repairRay = new Ray(sampleOrigin.transform.position, sampleOrigin.transform.forward);
                RaycastHit raycastHit;

                if (Physics.Raycast(repairRay, out raycastHit, sampleDistance))
                {
                    repairData = raycastHit.collider.GetComponent<RepairData>();
                    if (repairData != null)
                    {
                        if (repairData.repaired)
                        {
                            UIController.instance.ChangeTempText("Already Repaired");
                        }
                        else
                        {
                            StartAction(RoverAction.Repair);
                        }
                    }

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && groundControlActive)
        {
            UIController.instance.ToggleStream();
        }

        if (potentialPhoto)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Quest cameraQuest = QuestController.instance.ActiveQuestOfType(typeof(CameraQuest));
                cameraQuest.questData.success = potentialPhotoCorrect;

                if (cameraQuest.tutorialQuest)
                {
                    if (potentialPhotoCorrect)
                    {
                        cameraQuest.EndQuest();
                    }
                    else
                    {
                        UIController.instance.ShowTutorialText("Space Station Not In View, Try Again");
                    }

                    HidePhoto();
                }
                else
                {
                    cameraQuest.EndQuest();
                }
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                HidePhoto();
            }
        }

        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        if (outOfBattery || freezeMovement)
        {
            movementInput = Vector2.zero;
        }
        else
        {
            if (GameController.instance != null)
            {
                if (GameController.instance.IsPaused())
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
            }
        }

        wheelDrive.SetMoveInput(movementInput);

        fpsCamera.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);

        stats.currentPosition = transform.position;

    }

    public enum RoverAction { Repair, Sample };

    IEnumerator ProgressAction(RoverAction roverAction)
    {
        switch (roverAction)
        {
            case (RoverAction.Repair):
                while (true)
                {
                    actionProgress += Time.deltaTime;
                    float percent = Mathf.InverseLerp(0f, repairData.repairTime, actionProgress);

                    if (UIController.instance != null)
                    {
                        UIController.instance.UpdateProgressBar(percent);
                    }

                    if (Input.GetMouseButtonUp(1) || percent >= .99f)
                    {
                        FinishAction(roverAction, percent >= .99f);
                        yield break;
                    }
                    yield return null;
                }

            case (RoverAction.Sample):
                while (true)
                {
                    actionProgress += Time.deltaTime;
                    float percent = Mathf.InverseLerp(0f, sampleData.sampleTime, actionProgress);

                    if (UIController.instance != null)
                    {
                        UIController.instance.UpdateProgressBar(percent);
                    }

                    if (Input.GetMouseButtonUp(0) || percent >= .99f)
                    {
                        FinishAction(roverAction, percent >= .99f);
                        yield break;
                    }
                    yield return null;
                }
        }
        yield return null;
    }

    void StartAction(RoverAction roverAction)
    {
        if (!actionInProgress)
        {
            actionInProgress = true;
            actionProgress = 0f;

            StartCoroutine(ProgressAction(roverAction));
        }
    }

    void FinishAction(RoverAction roverAction, bool complete)
    {
        actionInProgress = false;

        if (complete && QuestController.instance != null)
        {
            switch (roverAction)
            {
                case (RoverAction.Repair):
                    {
                        repairData.repaired = true;
                        UIController.instance.ChangeTempText("Repair Successful");
                    }
                    break;

                case (RoverAction.Sample):
                    {
                        UIController.instance.ChangeTempText("Sample Results: " + sampleData.sampleType.ToString());
                        QuestController.instance.SendSample(sampleData);
                    }
                    break;
            }
        }
    }

    public void ToggleCameraMode(bool on)
    {
        cameraMode = on;

        Quest cameraQuest = QuestController.instance.ActiveQuestOfType(typeof(CameraQuest));

        if (cameraMode)
        {
            if (cameraQuest != null && cameraQuest.tutorialQuest)
            {
                UIController.instance.ShowTutorialText("Press Left Mouse Button to Capture Photo");
            }
        }
        else
        {
            fpsCamera.fieldOfView = 90f;
            FreezeRoverStates(FreezeType.Movement, false);
        }
    }

    public void ShowPhoto(Texture2D photo, bool correct)
    {
        potentialPhoto = true;
        potentialPhotoCorrect = correct;

        UIController.instance.ShowNewPhoto(photo);
    }

    public void HidePhoto()
    {
        potentialPhoto = false;
        potentialPhotoCorrect = false;

        UIController.instance.DeleteNewPhoto();
    }

    public enum FreezeType { Battery, Movement, Physics, All};

    public void FreezeRoverStates(FreezeType freezeType, bool freeze)
    {
        switch(freezeType)
        {
            case (FreezeType.All):
                {
                    freezeMovement = freeze;
                    freezeInPlace = freeze;
                    freezeBattery = freeze;
                    break;
                }

            case (FreezeType.Battery):
                {
                    freezeBattery = freeze;
                    break;
                }

            case (FreezeType.Movement):
                {
                    freezeMovement = freeze;
                    break;
                }

            case (FreezeType.Physics):
                {
                    freezeInPlace = freeze;
                    break;
                }
        }
    }
}

[System.Serializable]
public struct RoverStats
{
    public bool newSave;
    public int saveFileIndex;
    public int savedLevel;
    public float batteryLife;
    public float maxBattery;
    public Vector3 lastSavedPosition;
    public Vector3 currentPosition;

}
