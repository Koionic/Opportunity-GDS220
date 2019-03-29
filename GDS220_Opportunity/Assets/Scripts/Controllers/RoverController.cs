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
    float minimumY = -60f, maximumY = 60f;

    float rotationX;
    float rotationY;

    public Camera fpsCamera;

    Rigidbody rb;

    public UnityEvent photoTaken;

    public bool cameraMode;

    public UnityEvent OutOfBattery;

    public bool freezeMovement = true;
    public bool freezeInPlace = true;
    public bool freezeBattery = true;

    bool outOfBattery = false;

    [SerializeField]
    float batteryDepleteRate = .5f;

    public RoverStats stats;

    void Start()
    {
        wheelDrive = GetComponent<WheelDrive>();
        rb = GetComponent<Rigidbody>();

        if (LevelDataHolder.instance != null)
            stats.savedLevel = LevelDataHolder.instance.currentLevel;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        ProcessInputs();

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
            cameraMode = !cameraMode;

            if (cameraMode)
            {
                FreezeRoverStates(FreezeType.Movement, true);
            }
            else
            {
                fpsCamera.fieldOfView = 90f;
                FreezeRoverStates(FreezeType.Movement, false);
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
