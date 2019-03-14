using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    WheelDrive wheelDrive;

    [SerializeField]
    float sensitivityX = 15f, sensitivityY = 15f;

    [SerializeField]
    float minimumX = -360f, maximumX = 360f;

    [SerializeField]
    float minimumY = -60f, maximumY = 60f;

    float rotationX;
    float rotationY;

    [SerializeField]
    Camera fpsCamera;

    [SerializeField]
    float mouseLookLimitX, mouseLookLimitY;

    // Start is called before the first frame update
    void Start()
    {
        wheelDrive = GetComponent<WheelDrive>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessInputs();
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

    }
}
