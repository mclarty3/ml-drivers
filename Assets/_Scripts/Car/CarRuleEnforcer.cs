using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRuleEnforcer : MonoBehaviour
{
    CarPercepts carPercepts;
    CarController carController;
    PathCrawler pathCrawler;

    [SerializeField][Tooltip("The distance at which a car is considered to have reached a traffic signal")]
    private float _reachedTrafficSignalDistance = 0.5f;
    [SerializeField][Tooltip("The distance at which a car is considered to have passed a traffic signal")]
    private float _passedTrafficSignalDistance = 0.3f;
    [SerializeField][Tooltip("The minimum amount of time a car must wait at a stop sign before it can pass")]
    private float _minStopSignStopTime = 1.5f;

    public Vector3 _trafficSignalPosition = Vector3.zero;
    public NodePath _trafficSignalPath = null;
    private int _trafficSignalType;
    private float _distToSignal;
    private bool _reachedTrafficSignal = false;
    private float _stopSignTimer;
    private bool _ranTrafficSignal = false;

    // Start is called before the first frame update
    void Start()
    {
        carPercepts = GetComponent<CarPercepts>();
        carController = GetComponent<CarController>();
        pathCrawler = GetComponent<PathCrawler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_trafficSignalPosition == Vector3.zero)
        {
            CheckForTrafficSignal();
        }
        else
        {
            _distToSignal = Vector3.Distance(_trafficSignalPosition, transform.position);
            _trafficSignalType = _trafficSignalPath.connectedTrafficSignal.signalType;

            if (!_reachedTrafficSignal)
            {
                if (_distToSignal != -1)
                {
                    CheckReachedTrafficSignal();

                    if (_reachedTrafficSignal && _trafficSignalType == 3)
                    {
                        _stopSignTimer = Time.time;
                    }
                }
            }
            else
            {
                if (_trafficSignalType != 0 && _trafficSignalType != 3)
                {
                    ResetTrafficSignal();
                }
                else if (_trafficSignalType == 0)
                {
                    if (CheckRanTrafficSignal())
                    {
                        ResetTrafficSignal();
                    }
                }
                else if (_trafficSignalType == 3)
                {
                    if (Time.time - _stopSignTimer >= _minStopSignStopTime)
                    {
                        ResetTrafficSignal();
                    }
                    else if (CheckRanTrafficSignal())
                    {
                        ResetTrafficSignal();
                    }
                }
            }
        }
    }

    public bool CheckRanTrafficSignal(bool clear = false)
    {
        if (_ranTrafficSignal)
        {
            if (clear)
            {
                _ranTrafficSignal = false;
            }
            return true;
        }
        return false;
    }

    private void CheckForTrafficSignal()
    {
        if (carPercepts.CheckStopForTrafficSignal(out float distance) && pathCrawler.currentPath.connectedTrafficSignal != null)
        {
            _trafficSignalPath = pathCrawler.currentPath;
            _trafficSignalPosition = carPercepts.GetTrafficSignalNodePosition();
            _trafficSignalType = carPercepts.approachingTrafficSignalType;
            _distToSignal = distance;
        }
        else
        {
            _trafficSignalPath = null;
            _trafficSignalPosition = Vector3.zero;
            _trafficSignalType = -1;
            _distToSignal = -1;
        }
    }

    private void ResetTrafficSignal()
    {
        _trafficSignalPath = null;
        _trafficSignalPosition = Vector3.zero;
        _trafficSignalType = -1;
        _distToSignal = -1;
        _reachedTrafficSignal = false;
    }

    private void CheckReachedTrafficSignal()
    {
        if (_distToSignal != -1 && _distToSignal <= _reachedTrafficSignalDistance)
        {
            _reachedTrafficSignal = true;
        }
        else
        {
            _reachedTrafficSignal = false;
        }
    }

    private bool CheckRanTrafficSignal()
    {
        bool isBehind = Vector3.Dot(transform.forward, _trafficSignalPosition - transform.position) < 0;
        if (_distToSignal != -1 && _distToSignal >= _passedTrafficSignalDistance && isBehind)
        {
            return true;
        }
        return false;
    }

}
