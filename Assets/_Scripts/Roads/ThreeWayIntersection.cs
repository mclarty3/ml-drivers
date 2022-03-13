using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ThreeWayIntersection : RoadPiece
{
    // Start is called before the first frame update
    void Start()
    {
        if (roadConnections.Count != 3)
        {
            Debug.LogError("ThreeWayIntersection must have exactly three road connections");
        }
        foreach (RoadConnection connection in roadConnections)
        {
            if (connection.outPaths.Count != 2 || connection.inPaths.Count != 2)
            {
                Debug.LogError("ThreeWayIntersection must have exactly two out and two in paths for each road connection");
            }
        }
    }

    public override void AddTrafficLight()
    {
        GameObject prefab = LevelManager.GetInstance().prefabDict["ThreeWayTrafficLight"];
        GameObject trafficSignal = Instantiate(prefab, transform.position, transform.rotation);
        trafficSignal.transform.parent = transform;
        tsm = trafficSignal.GetComponent<TrafficSignalManager>();

        roadConnections[0].trafficSignalGroup = tsm.trafficSignalGroups[0];
        roadConnections[1].trafficSignalGroup = tsm.trafficSignalGroups[1];
        roadConnections[2].trafficSignalGroup = tsm.trafficSignalGroups[1];

        foreach (RoadConnection roadConnection in roadConnections) {
            if (roadConnection.connectedTo == null) continue;

            foreach (NodePath path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficSignal = roadConnection.trafficSignalGroup;
            }
        }
    }

    public override void AddStopSign()
    {
        GameObject prefab = LevelManager.GetInstance().prefabDict["ThreeWayStopSign"];
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 180, 0);
        GameObject stopSignIntersection = Instantiate(prefab, transform.position, rot);
        stopSignIntersection.transform.parent = transform;
        tsm = stopSignIntersection.GetComponent<TrafficSignalManager>();

        foreach (RoadConnection connection in roadConnections)
        {
            connection.trafficSignalGroup = tsm.trafficSignalGroups[0];
        }

        foreach (RoadConnection roadConnection in roadConnections) {
            roadConnection.trafficSignalGroup = tsm.trafficSignalGroups[0];

            if (roadConnection.connectedTo == null) continue;

            foreach (NodePath path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficSignal = roadConnection.trafficSignalGroup;
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
        else
        {
            return null;
        }
    }

    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go)
    {
        List<RoadConnection> connectedRoads = GetFullConnections();
        go = null;
        if (connectedRoads.Count == 3)
        {
            go = ConvertToFourWay(otherPiece);
        }
        else
        {
            foreach (RoadConnection conn in roadConnections)
            {
                if (conn.connectedTo == null)
                {
                    other.ConnectTo(conn);
                    conn.ConnectTo(other);
                }
            }
        }
        return null;
    }

    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false)
    {
        List<RoadConnection> connectedRoads = GetFullConnections();
        List<RoadConnection> placementConnections = toPlace.GetFullConnections();

        GameObject[] changedObjects = new GameObject[2] { null, null };

        Vector3 otherPos = toPlace.transform.position;
        Vector3 vector = otherPos - transform.position;
        if (connectedRoads.Count == 3)
        {
            GameObject newNodeVis = ConvertToFourWay(toPlace);
            if (!dontRepeat)
            {
                changedObjects[0] = newNodeVis;
            }
            else
            {
                changedObjects[1] = newNodeVis;
            }
            return changedObjects;
        }
        return null;
    }

    // public override GameObject HandleRoadRemoval(RoadPiece toRemove)
    // {
    //     List<RoadConnection> connectedRoads = GetFullConnections();
    //     List<RoadConnection> keepConnects = connectedRoads.FindAll(x => x.connectedTo.roadPiece != toRemove);

    //     GameObject straightPrefab = LevelManager.GetInstance().prefabDict["TwoDirectionRoad"];
    //     GameObject newRoad = Instantiate(straightPrefab, transform.position, transform.rotation);
    //     RoadPiece newPiece = newRoad.GetComponent<RoadPiece>();

    //     foreach (RoadConnection connection in keepConnects)
    //     {
    //         RoadConnection otherConnection = connection.connectedTo;
    //         otherConnection.connectedTo = null;
    //         RoadPiece otherPiece = otherConnection.roadPiece;
    //         Vector3 positionDiff = transform.position - otherPiece.transform.position;
    //         RoadConnection newConnect = newPiece.AddConnectionFromVector(positionDiff, otherConnection,
    //                                                                      otherPiece, out GameObject go);

    //     }

    //     Destroy(gameObject);
    //     return newRoad;
    // }

    public GameObject ReduceConnections(List<RoadConnection> keepConnections)
    {
        GameObject straightPrefab = LevelManager.GetInstance().prefabDict["TwoDirectionRoad"];
        GameObject straightRoad = Instantiate(straightPrefab, transform.position, transform.rotation);
        RoadPiece straightPiece = straightRoad.GetComponent<RoadPiece>();

        foreach (RoadConnection connection in keepConnections)
        {
            RoadConnection otherConnection = connection.connectedTo;
            otherConnection.connectedTo = null;
            RoadPiece otherPiece = otherConnection.roadPiece;
            Vector3 positionDiff = transform.position - otherPiece.transform.position;
            RoadConnection newConnect = straightPiece.AddConnectionFromVector(positionDiff,
                                                                              otherConnection,
                                                                              otherPiece,
                                                                              out GameObject newObj);

        }

        Destroy(gameObject);
        return straightRoad;
    }

    public GameObject ConvertToStraight(RoadConnection keepConnection)
    {
        GameObject straightPrefab = LevelManager.GetInstance().prefabDict["TwoDirectionRoad"];
        GameObject straightRoad = Instantiate(straightPrefab, transform.position, transform.rotation);
        RoadPiece straightPiece = straightRoad.GetComponent<RoadPiece>();

        for (int i = 1; i < 3; i++)
        {
            RoadConnection otherConnection = roadConnections[i].connectedTo;
            RoadPiece otherPiece = otherConnection.roadPiece;
            Vector3 positionDiff = transform.position - otherPiece.transform.position;
            RoadConnection newConnect = straightPiece.AddConnectionFromVector(positionDiff,
                                                                              otherConnection,
                                                                              otherPiece,
                                                                              out GameObject newObj);

        }

        Destroy(gameObject);
        return straightRoad;
    }

    public GameObject ConvertToElbow(RoadConnection keepConnection, bool flip=false)
    {
        GameObject elbowPrefab = LevelManager.GetInstance().prefabDict["ElbowRoad"];
        GameObject elbowRoad = Instantiate(elbowPrefab, transform.position, transform.rotation);
        return null;
    }

    public GameObject ConvertToFourWay(RoadPiece newPiece)
    {
        Vector3 toNewPiece = newPiece.transform.position - transform.position;
        GameObject fourWayPrefab = LevelManager.GetInstance().prefabDict["FourWayIntersection"];
        GameObject newRoad = Instantiate(fourWayPrefab, transform.position,
                                         transform.rotation);

        FourWayIntersection fourWay = newRoad.GetComponent<FourWayIntersection>();
        for (int i = 0; i < 3; i++)
        {
            fourWay.roadConnections[i].ConnectTo(roadConnections[i].connectedTo);
            roadConnections[i].connectedTo.ConnectTo(fourWay.roadConnections[i]);
        }
        RoadConnection newConnect = newPiece.AddConnectionFromVector(toNewPiece,
                                                                     fourWay.roadConnections[3],
                                                                     this, out GameObject go);
        // fourWay.roadConnections[3].ConnectTo(newConnect);
        Destroy(gameObject);
        return newRoad;
    }
}
