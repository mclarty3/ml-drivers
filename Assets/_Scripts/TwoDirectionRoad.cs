using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class TwoDirectionRoad : RoadPiece
{
    public bool elbowRoad = false;

    public static GameObject straightPrefab = null;
    public static GameObject elbowPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        if (straightPrefab == null)
        {  
            straightPrefab = Resources.Load<GameObject>("Prefabs/TwoDirectionRoad");
        }
        if (elbowPrefab == null)
        {
            elbowPrefab = Resources.Load<GameObject>("Prefabs/ElbowRoad");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override RoadConnection GetRoadConnectionFromVector(Vector3 vector)
    {
        vector = vector.normalized;
        if (-vector == transform.forward)
        {
            return roadConnections[0];
        }
        else if (vector == transform.forward)
        {
            return roadConnections[1];
        }
        else
        {
            return null;
        }
    }

    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go)
    {
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        go = null;
        if (connectedRoads.Count == 2)
        {
            go = ConvertToThreeWay(otherPiece);
            return null;
        }
        else if (connectedRoads.Count == 1)
        {
            Vector3 otherPos = connectedRoads[0].transform.position;
            if (Vector3.Dot((otherPos - transform.position).normalized, vector.normalized) == 1)
            {
                RoadConnection connection = GetRoadConnectionFromVector(vector);
                connection.ConnectTo(other);
                return connection;
            }
            else if (elbowRoad)
            {
                RoadConnection connection = null;
                foreach (RoadConnection conn in roadConnections)
                {
                    if (conn.connectedTo == null)
                    {
                        connection = conn;
                        break;
                    }
                }
                if (connection != null)
                {
                    connection.ConnectTo(other);
                    other.ConnectTo(connection);
                    return connection;
                }
            }
            else
            {
                go = ConvertToElbow(otherPiece);
                return null;
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-vector, transform.up); 
            roadConnections[0].ConnectTo(other);
            // roadConnections[0].connectedTo = other;
            return roadConnections[0];
        }

        return null;
    }

    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false)
    {
        // Called on surrounding roads when a new road is placed in an adjacent tile
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        
        GameObject[] changedObjects = new GameObject[2] { null, null };
        GameObject newPlacement = null;

        Vector3 otherPos = toPlace.transform.position;
        Vector3 toOtherPos = otherPos - transform.position;
        // If this road is an unconnected straight road
        if (connectedRoads.Count == 0)
        {
            transform.rotation = Quaternion.LookRotation(toOtherPos, transform.up);
            RoadConnection connect;
            transform.rotation = Quaternion.LookRotation(toOtherPos, transform.up);
            connect = roadConnections[0];
            RoadConnection newConnect = toPlace.AddConnectionFromVector(toOtherPos, connect, this, 
                                                                     out newPlacement);
            if (newConnect != null)
            {
                connect.ConnectTo(newConnect);
            }
            if (newPlacement != null)
            {
                changedObjects[1] = newPlacement;
            }
            return changedObjects;
        }
        else if (connectedRoads.Count == 1)
        {
            float angle = Vector3.Angle(toOtherPos, transform.forward);
            if (angle == 180 || angle == 0)
            {
                RoadConnection thisConnect = GetRoadConnectionFromVector(-toOtherPos);
                RoadConnection otherConnect = toPlace.AddConnectionFromVector(toOtherPos, thisConnect, 
                                                                              this, out newPlacement);
                if (otherConnect != null)
                {
                    thisConnect.ConnectTo(otherConnect);
                }
                if (newPlacement != null)
                {
                    changedObjects[1] = newPlacement;
                }
                return changedObjects;
            }
            else
            {
                GameObject newNodeVis = ConvertToElbow(toPlace);
                changedObjects[dontRepeat ? 1 : 0] = newNodeVis;
                return changedObjects;
            }
        }
        else if (connectedRoads.Count == 2)
        {
            GameObject newNodeVis = ConvertToThreeWay(toPlace);
            changedObjects[dontRepeat ? 1 : 0] = newNodeVis;
            return changedObjects;
        }
        return changedObjects;
    }

    public GameObject ConvertToThreeWay(RoadPiece newPiece)
    {
        Vector3 toNewPiece = newPiece.transform.position - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(toNewPiece, transform.up);
        GameObject newRoad;
        GameObject go;
        GameObject threeWayPrefab = LevelManager.GetInstance().prefabDict["ThreeWayIntersection"];
        if (!elbowRoad)
        {
            newRoad = Instantiate(threeWayPrefab, transform.position, 
                                            newRotation);
            int xPlusIndex = 0;
            int xMinusIndex = 1;
            float angle = Convert.ToInt16(Vector3.SignedAngle(newRoad.transform.forward, 
                                                              transform.forward, transform.up));
            if (angle != 90)
            {
                xPlusIndex = 1;
                xMinusIndex = 0;   
            }
            ThreeWayIntersection threeWay = newRoad.GetComponent<ThreeWayIntersection>();
            threeWay.roadConnections[1].ConnectTo(roadConnections[xPlusIndex].connectedTo);
            roadConnections[xPlusIndex].connectedTo.ConnectTo(threeWay.roadConnections[1]);
            threeWay.roadConnections[2].ConnectTo(roadConnections[xMinusIndex].connectedTo);
            roadConnections[xMinusIndex].connectedTo.ConnectTo(threeWay.roadConnections[2]);
            RoadConnection newConnect = newPiece.AddConnectionFromVector(toNewPiece, 
                                                                         threeWay.roadConnections[0], 
                                                                         threeWay, out go);
            if (newConnect != null)
            {
                threeWay.roadConnections[0].ConnectTo(newConnect);
            }
        }
        else
        {
            float angle = Convert.ToInt16(Vector3.Angle(transform.forward, toNewPiece));
            // New  road is perpendicular relative to Z+ of elbow
            int zPlusIndex = 0;
            int xIndex = 2;
            int newIndex = 1;
            Quaternion rot = Quaternion.LookRotation(transform.forward, transform.up);
            if (angle != 90)
            {
                zPlusIndex = 1;
                xIndex = 0;
                newIndex = 2;
                rot = Quaternion.LookRotation(-transform.right, transform.up);
            }
            newRoad = Instantiate(threeWayPrefab, transform.position, rot);
            ThreeWayIntersection threeWay = newRoad.GetComponent<ThreeWayIntersection>();
            RoadConnection newConnect = newPiece.AddConnectionFromVector(toNewPiece, 
                                                                         threeWay.roadConnections[newIndex], 
                                                                         threeWay, out go);
            if (newConnect != null)
            {
                threeWay.roadConnections[newIndex].ConnectTo(newConnect);
            }
            threeWay.roadConnections[zPlusIndex].ConnectTo(roadConnections[0].connectedTo);
            roadConnections[0].connectedTo.ConnectTo(threeWay.roadConnections[zPlusIndex]);
            threeWay.roadConnections[xIndex].ConnectTo(roadConnections[1].connectedTo);
            roadConnections[1].connectedTo.ConnectTo(threeWay.roadConnections[xIndex]);
        }
        Destroy(gameObject);

        return newRoad;
    }

    public GameObject ConvertToElbow(RoadPiece newPiece)
    {
        RoadConnection connection = null;
        GameObject go;
        if (roadConnections[0].connectedTo != null)
        {
            connection = roadConnections[0];
        }
        else
        {
            connection = roadConnections[1];
        }
        Vector3 toConnect = (connection.transform.position - transform.position).normalized;
        Vector3 toNewPiece = newPiece.transform.position - transform.position;
        float angle = Convert.ToInt16(Vector3.SignedAngle(toConnect, toNewPiece, transform.up));

        // Assumes right turn
        Quaternion rot = Quaternion.LookRotation(toConnect, transform.up);
        int currentConnectionIndex = 0;
        int newConnectionIndex = 1;
        if (angle == 90)
        {
            rot = Quaternion.LookRotation(toNewPiece, transform.up);
            currentConnectionIndex = 1;
            newConnectionIndex = 0;
        }
        GameObject newRoad = Instantiate(TwoDirectionRoad.elbowPrefab, transform.position, rot);
        TwoDirectionRoad elbow = newRoad.GetComponent<TwoDirectionRoad>();
        elbow.roadConnections[currentConnectionIndex].ConnectTo(connection.connectedTo);
        connection.connectedTo.ConnectTo(elbow.roadConnections[currentConnectionIndex]);
        RoadConnection newConnect = newPiece.AddConnectionFromVector(toNewPiece, 
                                                                     elbow.roadConnections[newConnectionIndex], 
                                                                     elbow, out go);
        if (newConnect != null)
        {
            elbow.roadConnections[newConnectionIndex].ConnectTo(newConnect);
        }
        Destroy(gameObject);

        return newRoad;
    }
}
