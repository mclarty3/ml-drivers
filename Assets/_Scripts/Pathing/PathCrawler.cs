using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCrawler : MonoBehaviour
{
    public NodePath currentPath;
    public int currentNodeIndex;
    public float speed = 0.5f;
    public float nodeTriggerDistance = 0.1f;
    public float timeBetweenNodes = 2f;
    public float avoidCarDistanceMultiplier = 0.1f;
    public float minAvoidCarDistance = 1f;
    public float beginBrakeForTrafficSignalDistance = 1f;
    public float trafficSignalBrakeDistanceMultiplier = 1f;
    public float trafficSignalBrakeIntensity = 0.5f;
    public float maxVelocity = 15f;
    public Vector3 currentNodePosition;

    public bool _stoppingAtTrafficSignal;
    public bool _waitingAtTrafficSignal;
    private bool _waited = false;
    private int _signalType;
    private float _stopSignTime;

    CarPercepts carPercepts;
    CarController carController;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        transform.position = new Vector3(nodePos.x, transform.position.y, nodePos.z);
        MoveToNextNode();
        Vector3 rot = Quaternion.LookRotation(currentPath.nodes[currentNodeIndex + 1] - transform.position).eulerAngles;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);


        carPercepts = GetComponent<CarPercepts>();
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(0, 0.5f, 0);
        Debug.DrawLine(transform.position + offset, currentNodePosition + offset, Color.red);

        if (currentPath == null) {
            return;
        }

        float steerAngle = GetSteerToNextNode();
        steerAngle = Mathf.Clamp(steerAngle / carController.maxSteerAngle, -1, 1);

        if (CarInFront(out bool emergency)) {
            float brake = carController.velocity < 0 ? -1 : 0;
            SetCarControllerInput(steerAngle, brake);
        }

        if (emergency)
        {
            SetCarControllerInput(steerAngle, carController.velocity < 0 ? -1 : 0);
            return;
        }

        if (carPercepts.CheckStopForTrafficSignal(out float distanceToTrafficSignal) && !_waitingAtTrafficSignal
            && !_stoppingAtTrafficSignal && !_waited)
        {
            if (distanceToTrafficSignal != -1 && distanceToTrafficSignal < GetBrakeDistance())
            {
                _stoppingAtTrafficSignal = true;
                _waited = false;
                _signalType = currentPath.connectedTrafficSignal.signalType;

                if (_signalType == 3)
                {
                    _stopSignTime = Time.time;
                }
            }
        }

        if (_stoppingAtTrafficSignal)
        {
            _signalType = currentPath.connectedTrafficSignal.signalType;
            if (_signalType != 3 && _signalType != 0)
            {
                _stoppingAtTrafficSignal = false;
                MoveToNextPath();
            }
            else if (carController.velocity == 0)
            {
                _stoppingAtTrafficSignal = false;
                _waitingAtTrafficSignal = true;
            }
            else
            {
                float brake = 0;
                if (distanceToTrafficSignal < GetBrakeDistance() && carController.velocity > 0)
                {
                    brake = -1 * trafficSignalBrakeIntensity / distanceToTrafficSignal;
                    brake *= Mathf.Pow(carController.velocity, 2);
                }
                brake = Mathf.Clamp(brake, -1, 0);
                SetCarControllerInput(0, brake);
                return;
            }
        }

        if (_waitingAtTrafficSignal)
        {
            _signalType = currentPath.connectedTrafficSignal.signalType;
            if ((_signalType == 3 && Time.time - _stopSignTime >= 3) ||
                (_signalType != 3 && _signalType != 0))
            {
                _waitingAtTrafficSignal = false;
                _waited = true;
                MoveToNextPath();
            }
            SetCarControllerInput(steerAngle, 0);
            return;
        }

        if (Vector3.Distance(transform.position, currentNodePosition) <=  nodeTriggerDistance) {
            MoveToNextNode();
            steerAngle = GetSteerToNextNode();
        }

        float accel = carController.velocity > maxVelocity ? 0 : 1;
        SetCarControllerInput(steerAngle, accel);
    }

    public void MoveToNextNode()
    {
        currentNodeIndex++;
        if (currentNodeIndex >= currentPath.NumNodes)
        {
            _waited = false;
            currentNodeIndex = 0;
            currentPath = currentPath.GetConnectingPath();
        }
        Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        currentNodePosition = new Vector3(nodePos.x, transform.position.y, nodePos.z);
    }

    void MoveToNextPath()
    {
        currentPath = currentPath.GetConnectingPath();
        currentNodeIndex = 0;
        currentNodePosition = currentPath.nodes[currentNodeIndex];
    }

    private float GetBrakeDistance()
    {
        return 1 * (carController.velocity / 10) * trafficSignalBrakeDistanceMultiplier;
    }

    void InitiateStopForTrafficSignal()
    {
        _waitingAtTrafficSignal = true;
    }

    private void SetCarControllerInput(float horizontal, float vertical)
    {
        carController.SetInput(vertical, horizontal);
    }

    private float GetSteerToNextNode()
    {
        Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        float angle = Vector3.SignedAngle(transform.forward, nodePos - transform.position, transform.up);
        return angle;
    }

    bool CarInFront(out bool emergency)
    {
        List<float> distances;
        emergency = false;
        if (carPercepts.GetCollisions(out distances, "Car"))
        {
            for (int i = 0; i < 9; i++)
            {
                try {
                    if (distances[i] < minAvoidCarDistance)
                    {
                        emergency = true;
                        return true;
                    }
                    if (distances[i] <= avoidCarDistanceMultiplier * Mathf.Pow(carController.velocity, 2))
                    {
                        return true;
                    }
                } catch (System.Exception) {
                    Debug.LogError("SOMETHING BAD");
                }
            }
        }
        return false;
    }
}
