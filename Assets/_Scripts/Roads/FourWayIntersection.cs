using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class FourWayIntersection : RoadPiece
{
    TrafficSignalManager tsm;
    private int _currentSignalType = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (roadConnections.Count != 4)
        {
            Debug.LogError("FourWayIntersection must have exactly four road connections");
        }
        foreach (RoadConnection connection in roadConnections)
        {
            if (connection.outPaths.Count != 3 || connection.inPaths.Count != 3)
            {
                Debug.LogError("FourWayIntersection must have exactly three out and three in paths for "
                                + "each road connection");
            }
        }
        AddStopSign();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CycleTrafficSignal(int signalType = -1)
    {
        RemoveTrafficSignal();
        switch (signalType)
        {
            case 0:
                _currentSignalType = 0;
                break;
            case 1:
                AddTrafficLight();
                _currentSignalType = 1;
                break;
            case 2:
                AddStopSign();
                _currentSignalType = 2;
                break;
            default:
                switch (_currentSignalType)
                {
                    case 0:
                        AddTrafficLight();
                        _currentSignalType = 1;
                        break;
                    case 1:
                        AddStopSign();
                        _currentSignalType = 2;
                        break;
                    case 2:
                    	_currentSignalType = 0;
                        break;
                }
                break;
        }
        Debug.Log("Set signal type to " + _currentSignalType);
    }

    public void AddTrafficLight()
    {
        GameObject prefab = LevelManager.GetInstance().prefabDict["FourWayTrafficLight"];
        GameObject trafficSignal = Instantiate(prefab, transform.position, transform.rotation);
        trafficSignal.transform.parent = transform;
        tsm = trafficSignal.GetComponent<TrafficSignalManager>();

        roadConnections[1].trafficSignalGroup = tsm.trafficSignalGroups[0];
        roadConnections[2].trafficSignalGroup = tsm.trafficSignalGroups[0];
        roadConnections[0].trafficSignalGroup = tsm.trafficSignalGroups[1];
        roadConnections[3].trafficSignalGroup = tsm.trafficSignalGroups[1];

        foreach (RoadConnection roadConnection in roadConnections) {
            if (roadConnection.connectedTo == null) continue;

            foreach (Path path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficSignal = roadConnection.trafficSignalGroup;
            }
        }
    }

    public void AddStopSign()
    {
        GameObject prefab = LevelManager.GetInstance().prefabDict["FourWayStopSign"];
        GameObject stopSignIntersection = Instantiate(prefab, transform.position, transform.rotation);
        stopSignIntersection.transform.parent = transform;
        tsm = stopSignIntersection.GetComponent<TrafficSignalManager>();

        foreach (RoadConnection roadConnection in roadConnections) {
            roadConnection.trafficSignalGroup = tsm.trafficSignalGroups[0];

            if (roadConnection.connectedTo == null) continue;

            foreach (Path path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficSignal = roadConnection.trafficSignalGroup;
            }
        }
    }

    public void RemoveTrafficSignal()
    {
        if (tsm == null) return;

        Destroy(tsm.gameObject);
        tsm = null;

        foreach (RoadConnection roadConnection in roadConnections) {
            roadConnection.trafficSignalGroup = null;

            if (roadConnection.connectedTo == null) continue;

            foreach (Path path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficSignal = null;
            }
        }
    }

    public override RoadConnection GetRoadConnectionFromVector(Vector3 vector)
    {
        vector = -vector.normalized;
        if (vector == transform.forward)
        {
            return roadConnections[0];
        }
        else if (vector == transform.right)
        {
            return roadConnections[1];
        }
        else if (vector == -transform.right)
        {
            return roadConnections[2];
        }
        else if (vector == -transform.forward)
        {
            return roadConnections[3];
        }
        else
        {
            return null;
        }
    }

    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go)
    {
        go = null;
        return null;
    }

    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false)
    { return null; }
}
