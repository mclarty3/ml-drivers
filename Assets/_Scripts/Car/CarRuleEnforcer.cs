using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRuleEnforcer : MonoBehaviour
{
    CarPercepts carPercepts;
    CarController carController;

    [SerializeField][Tooltip("The distance at which a car is considered to have reached a traffic signal")]
    private float _reachedTrafficSignalDistance = 0.4f;
    [SerializeField][Tooltip("The distance at which a car is considered to have passed a traffic signal")]
    private float _passedTrafficSignalDistance = 0.5f;
    [SerializeField][Tooltip("The minimum amount of time a car must wait at a stop sign before it can pass")]
    private float _minStopSignStopTime = 1.5f;

    Vector3 _trafficSignalPosition;
    int _trafficSignalType;
    public bool _reachedTrafficSignal = false;
    public float _distToSignal;
    float _stopSignTimer;

    // Start is called before the first frame update
    void Start()
    {
        carPercepts = GetComponent<CarPercepts>();
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_reachedTrafficSignal)
        {
            _distToSignal = Vector3.Distance(_trafficSignalPosition, transform.position);
            if (_trafficSignalType == 0)
            {
                if (carPercepts.approachingTrafficSignalType == 2)
                {
                    ResetTrafficSignal();
                }
                else if (CheckRanTrafficSignal())
                {
                    Debug.Log("I ran a stop light!!!!");
                    ResetTrafficSignal();
                }
            }
            else if (_trafficSignalType == 3)
            {
                if (Time.time - _stopSignTimer >= _minStopSignStopTime)
                {
                    _reachedTrafficSignal = false;
                    Debug.Log("Finished stop sign");
                    ResetTrafficSignal();
                }
                else if (CheckRanTrafficSignal())
                {
                    Debug.Log("I ran a stop sign!!!!");
                    ResetTrafficSignal();
                }
            }
        }
        else
        {
            CheckForTrafficSignal();
            if (_trafficSignalPosition != Vector3.zero && !_reachedTrafficSignal)
            {
                CheckReachedTrafficSignal();

                if (_reachedTrafficSignal && carPercepts.approachingTrafficSignalType == 3)
                {
                    _stopSignTimer = Time.time;
                }
            }
        }


    }

    private void CheckForTrafficSignal()
    {
        if (carPercepts.CheckStopForTrafficSignal(out float distance))
        {
            _trafficSignalPosition = carPercepts.GetTrafficSignalNodePosition();
            _trafficSignalType = carPercepts.approachingTrafficSignalType;
            _distToSignal = distance;
        }
        else
        {
            _trafficSignalPosition = Vector3.zero;
            _trafficSignalType = -1;
            _distToSignal = -1;
        }
    }

    private void ResetTrafficSignal()
    {
        _trafficSignalPosition = Vector3.zero;
        _trafficSignalType = -1;
        _distToSignal = -1;
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
        if (_distToSignal != -1 && _distToSignal >= _passedTrafficSignalDistance)
        {
            return true;
        }
        return false;
    }

}
