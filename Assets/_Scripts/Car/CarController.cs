using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW, rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT, rearDriverT, rearPassengerT;

    public float maxSteerAngle = 30;
    public float motorForce = 50;
    public float brakeForce = 100;
    public float velocityMultiplier = 10f;
    public float velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        Steer();
        AccelerateOrBrake();
        UpdateWheelPoses();
        UpdateVelocity();
    }

    public void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    public void SetInput(float horizontal, float vertical)
    {
        m_horizontalInput = horizontal;
        m_verticalInput = vertical;
    }

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;

        try {
            frontDriverW.steerAngle = m_steeringAngle;
            frontPassengerW.steerAngle = m_steeringAngle;
        } catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }
    }

    private void AccelerateOrBrake()
    {
        if (velocity == 0 || ((velocity > 0) == (m_verticalInput > 0)))
        {
            frontDriverW.brakeTorque = 0;
            frontPassengerW.brakeTorque = 0;
            frontDriverW.motorTorque = motorForce * m_verticalInput;
            frontPassengerW.motorTorque = motorForce * m_verticalInput;
        }
        else
        {
            frontDriverW.motorTorque = 0;
            frontPassengerW.motorTorque = 0;
            frontDriverW.brakeTorque = brakeForce * Mathf.Abs(m_verticalInput);
            frontPassengerW.brakeTorque = brakeForce * Mathf.Abs(m_verticalInput);
        }
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void UpdateVelocity()
    {
        velocity = GetComponent<Rigidbody>().velocity.magnitude * velocityMultiplier;
        velocity *= Mathf.Sign(Vector3.Dot(transform.forward, GetComponent<Rigidbody>().velocity));

        if (EqualsZero(velocity))
        {
            velocity = 0;
        }
    }

    private bool EqualsZero(float _value)
    {
        return Mathf.Abs(_value) < 0.05f;
    }
}
