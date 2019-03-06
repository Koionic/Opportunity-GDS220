using UnityEngine;
using System;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
    [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;

    [Tooltip("Steering angle of the wheels when turning")]
    public float turningAngle = 30f;
    [Tooltip("Maximum torque applied to the driving wheels")]
    public float maxTurningTorque = 300f;
    [Tooltip("How fast the vehicle can be going before turning")]
    public float turningBuffer = 0.1f;

    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

    private WheelCollider[] m_Wheels;

    bool turning, tryingToTurn;

    Rigidbody m_Rigidbody;

    // Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		m_Wheels = GetComponentsInChildren<WheelCollider>();

        m_Rigidbody = GetComponent<Rigidbody>();

		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
	}

	// This is a really simple approach to updating wheels.
	// We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
	// This helps us to figure our which wheels are front ones and which are rear.
	void Update()
	{
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		float angle = maxAngle * Input.GetAxis("Horizontal");
		float torque = maxTorque * Input.GetAxis("Vertical");

        float turningTorque = maxTurningTorque * Input.GetAxis("Horizontal");

		float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

        if (Input.GetAxis("Horizontal") != 0)
        {
            if (m_Rigidbody.velocity.magnitude <= turningBuffer)
            {
                turning = true;
                tryingToTurn = false;
            }
            else
            {
                turning = false;
                tryingToTurn = true;
            }
        }
        else
        {
            turning = false;
            tryingToTurn = false;
        }

        if (turning)
        {
            m_Wheels[0].steerAngle = turningAngle;
            m_Wheels[1].steerAngle = -turningAngle;

            m_Wheels[4].steerAngle = -turningAngle;
            m_Wheels[5].steerAngle = turningAngle;
        }


        foreach (WheelCollider wheel in m_Wheels)
		{
            /*
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;
            */


            if (turning)
            {
                wheel.brakeTorque = 0f;

                m_Rigidbody.constraints = RigidbodyConstraints.FreezePosition;

                if (wheel.transform.localPosition.x < 0)
                {
                    wheel.motorTorque = turningTorque;
                }
                else if (wheel.transform.localPosition.x > 0)
                {
                    wheel.motorTorque = -turningTorque;
                }
            }
            else
            {
                wheel.steerAngle = 0f;

                m_Rigidbody.constraints = RigidbodyConstraints.None;

                if (torque >= 0)
                {
                    wheel.brakeTorque = 0f;
                    wheel.motorTorque = torque;
                }
                else if (torque < 0)
                    wheel.brakeTorque = brakeTorque;

                if (tryingToTurn)
                    wheel.brakeTorque = brakeTorque;
            }

            // Update visual wheels if any.
            if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}
	}
}
