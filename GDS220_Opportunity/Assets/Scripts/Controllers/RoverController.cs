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

    [SerializeField]
    Camera fpsCamera;

    Rigidbody rb;

    public UnityEvent OutOfBattery;

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

        if (!outOfBattery)
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
            OutOfBattery.Invoke();
        }
    }

    void ChargeBattery(float charge)
    {
        stats.batteryLife = Mathf.Clamp((stats.batteryLife + charge), 0f, stats.maxBattery);
    }

    void ProcessInputs()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        wheelDrive.SetMoveInput(movementInput);

        fpsCamera.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        stats.currentPosition = transform.position;

    }

    public enum FreezeType { Battery, Movement, All};

    public void ToggleRoverStates(FreezeType freezeType, bool freeze)
    {
        switch(freezeType)
        {
            case (FreezeType.All):
                {
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
