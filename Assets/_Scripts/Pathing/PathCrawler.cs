using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCrawler : MonoBehaviour
{
    public Path currentPath;
    public int currentNodeIndex;
    public float speed = 0.5f;
    public float nodeTriggerDistance = 0.1f;
    public float timeBetweenNodes = 2f;
    public float avoidCarDistance = 0.5f;

    private Vector3 _currentNodePosition;
    public bool _waitingAtTrafficSignal;
    private bool _waited = false;
    private int _signalType;
    private float _stopSignTime;

    CarPercepts carPercepts;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        transform.position = new Vector3(nodePos.x, transform.position.y, nodePos.z);
        MoveToNextNode();

        carPercepts = GetComponent<CarPercepts>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPath == null) {
            return;
        }

        if (CarInFront()) {
            return;
        }

        if (_waitingAtTrafficSignal)
        {
            _signalType = currentPath.connectedTrafficSignal.signalType;
            if (_signalType == 3 && Time.time - _stopSignTime >= 3)
            {
                _waitingAtTrafficSignal = false;
                _waited = true;
            }
            else if (_signalType != 3 && _signalType != 0)
            {
                _waitingAtTrafficSignal = false;
                _waited = true;
            }
            return;
        }
        else if (Vector3.Distance(transform.position, _currentNodePosition) <=  nodeTriggerDistance) {
            MoveToNextNode();
        }

        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public void MoveToNextNode()
    {
        currentNodeIndex++;
        if (currentNodeIndex >= currentPath.NumNodes)
        {
            if (currentPath.connectedTrafficSignal != null && !_waited)
            {
                _signalType = currentPath.connectedTrafficSignal.signalType;
                if (_signalType == 0 || _signalType == 3)
                {
                    if (_signalType == 3)
                        _stopSignTime = Time.time;

                    _waitingAtTrafficSignal = true;
                    return;
                }
            }
            _waited = false;
            currentNodeIndex = 0;
            currentPath = currentPath.GetConnectingPath();
        }
        Vector3 nodePos = currentPath.nodes[currentNodeIndex];
        _currentNodePosition = new Vector3(nodePos.x, transform.position.y, nodePos.z);
        transform.rotation = Quaternion.LookRotation(_currentNodePosition - transform.position);
    }

    bool CarInFront()
    {
        List<float> distances;
        if (carPercepts.GetCollisions(out distances))
        {
            for (int i = 0; i < 15; i++)
            {
                if (distances[i] <= avoidCarDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
