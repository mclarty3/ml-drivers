using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCrawler : MonoBehaviour
{
    CarPercepts carPercepts;
    CarController carController;

    public bool heuristicControl = false;

    [Header("Path node tracking")]
    public NodePath currentPath;
    [HideInInspector]
    public Vector3 currentNodePosition = Vector3.zero;
    [HideInInspector]
    public Vector3[] nextThreeNodes;
    [HideInInspector]
    public int currentNodeIndex;

    [Header("Node traversal parameters")]
    [SerializeField]
    private float _nodeTriggerDistance = 0.1f;
    [SerializeField]
    private float _outOfLaneSidewaysDist = 0.7f;
    public float currentSideDist = 0;
    [SerializeField]
    private int _signalType;

    [Header("Heuristic driving parameters")]
    public float maxVelocity = 15f;
    [SerializeField]
    private float _avoidCarDistanceMultiplier = 0.1f;
    [SerializeField]
    private float _minAvoidCarDistance = 1f;
    [SerializeField]
    private float _trafficSignalBrakeDistanceMultiplier = 1f;
    [SerializeField]
    private float _trafficSignalBrakeIntensity = 0.5f;

    private bool _stoppingAtTrafficSignal;
    private bool _waitingAtTrafficSignal;
    private bool _waited = false;
    private float _stopSignTime;
    private NodePath _latestPath;
    private int _latestPathNodeIndex;
    private bool _changedNodes = false;

    // Start is called before the first frame update
    void Start()
    {
        carPercepts = GetComponent<CarPercepts>();
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(0, 0.5f, 0);
        Debug.DrawLine(transform.position + offset, currentNodePosition + offset, Color.red);
        for (int i = 0; i < 2; i++)
        {
            Debug.DrawLine(nextThreeNodes[i] + offset, nextThreeNodes[i + 1] + offset, Color.green);
        }

        if (!heuristicControl)
        {
            if (currentNodePosition != Vector3.zero)
            {
                if (Vector3.Distance(transform.position, currentNodePosition) < _nodeTriggerDistance)
                {
                    MoveToNextNode();
                }
            }
        }
        else
        {
            if (currentPath == null || currentPath.nodes == null) {
                Debug.LogWarning("No nodes! Can't drive");
                return;
            }

            float steerAngle = GetSteerToNextNode();
            steerAngle = Mathf.Clamp(steerAngle / carController.maxSteerAngle, -1, 1);

            if (CarInFront(out bool emergency)) {
                float brake = carController.velocity <= 0 ? 1 : 0;
                SetCarControllerInput(steerAngle, 0, brake);
            }

            if (emergency)
            {
                SetCarControllerInput(steerAngle, 0, carController.velocity <= 0 ? 1 : 0);
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
                        brake = 1 * _trafficSignalBrakeIntensity / distanceToTrafficSignal;
                        brake *= Mathf.Pow(carController.velocity, 2);
                    }
                    brake = Mathf.Clamp(brake, 0, 1);
                    SetCarControllerInput(0, 0, brake);
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
                SetCarControllerInput(steerAngle, 0, 1);
                return;
            }

            if (Vector3.Distance(transform.position, currentNodePosition) <=  _nodeTriggerDistance) {
                MoveToNextNode();
                steerAngle = GetSteerToNextNode();
            }

            float accel = carController.velocity > maxVelocity ? 0 : 1;
            SetCarControllerInput(steerAngle, accel, 0);
        }
    }

    public void Initialize(NodePath path)
    {
        currentPath = path;
        currentNodeIndex = 0;
        currentNodePosition = currentPath.nodes[currentNodeIndex];
        _latestPath = currentPath;
        _latestPathNodeIndex = currentNodeIndex;
        InitNextThreeNodes();
    }

    private void InitNextThreeNodes()
    {
        nextThreeNodes = new Vector3[3];
        nextThreeNodes[0] = GetNextNode(ref _latestPath, ref _latestPathNodeIndex);
        nextThreeNodes[1] = GetNextNode(ref _latestPath, ref _latestPathNodeIndex);
        nextThreeNodes[2] = GetNextNode(ref _latestPath, ref _latestPathNodeIndex);
    }

    private void UpdateNextThreeNodes()
    {
        nextThreeNodes[0] = nextThreeNodes[1];
        nextThreeNodes[1] = nextThreeNodes[2];
        nextThreeNodes[2] = GetNextNode(ref _latestPath, ref _latestPathNodeIndex);
    }

    public Vector3[] GetNextThreeNodes()
    {
        return nextThreeNodes;
    }

    public bool CheckChangedNodes(bool clear = false)
    {
        if (_changedNodes)
        {
            if (clear)
            {
                _changedNodes = false;
            }
            return true;
        }
        return false;
    }

    public bool IsInOtherLane()
    {
        Vector3 pathTangent = (nextThreeNodes[0] - currentNodePosition).normalized;
        Vector3 distToNode = currentNodePosition - transform.position;
        distToNode = new Vector3(distToNode.x, transform.position.y, distToNode.z);
        float sidewaysDist = Vector3.ProjectOnPlane(distToNode, pathTangent).magnitude;
        currentSideDist = sidewaysDist;
        return sidewaysDist >= _outOfLaneSidewaysDist;
    }

    private Vector3 GetNextNode(ref NodePath currentEndPath, ref int index)
    {
        index++;
        if (index >= currentEndPath.NumNodes)
        {
            index = 0;
            currentEndPath = currentEndPath.GetConnectingPath();
        }
        return currentEndPath.nodes[index];
    }

    public void MoveToNextNode()
    {
        // currentNodeIndex++;
        // if (currentNodeIndex >= currentPath.NumNodes)
        // {
        //     currentNodeIndex = 0;
        //     currentPath = currentPath.GetConnectingPath();
        // }
        // Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        // currentNodePosition = new Vector3(nodePos.x, transform.position.y, nodePos.z);
        currentNodePosition = nextThreeNodes[0];
        UpdateNextThreeNodes();
        if (_latestPathNodeIndex == 0)
        {
            currentPath = _latestPath;
        }
    }

    void MoveToNextPath()
    {
        currentPath = currentPath.GetConnectingPath();
        currentNodeIndex = 0;
        currentNodePosition = currentPath.nodes[currentNodeIndex];
    }

    private float GetBrakeDistance()
    {
        return 1 * (carController.velocity / 10) * _trafficSignalBrakeDistanceMultiplier;
    }

    void InitiateStopForTrafficSignal()
    {
        _waitingAtTrafficSignal = true;
    }

    private void SetCarControllerInput(float horizontal, float vertical, float brake)
    {
        carController.SetInput(vertical, horizontal, brake);
    }

    private float GetSteerToNextNode()
    {
        float angle = Vector3.SignedAngle(transform.forward, currentNodePosition- transform.position,
                                          transform.up);
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
                if (distances[i] != -1 && distances[i] < _minAvoidCarDistance)
                {
                    emergency = true;
                    return true;
                }
                if (distances[i] != -1 &&
                    distances[i] <= _avoidCarDistanceMultiplier * Mathf.Pow(carController.velocity, 2))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
