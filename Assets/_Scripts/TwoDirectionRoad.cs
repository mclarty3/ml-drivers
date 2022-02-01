using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class TwoDirectionRoad : RoadPiece
{
    bool elbowRoad = false;
    // public new List<RoadConnection> roadConnections = new List<RoadConnection>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override RoadConnection GetRoadConnectionFromVector(Vector3 vector)
    {
        if (-vector == Vector3.forward)
        {
            return roadConnections[0];
        }
        else if (vector == Vector3.forward)
        {
            return roadConnections[1];
        }
        else
        {
            return null;
        }
    }

    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other)
    {
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        if (connectedRoads.Count == 2)
        {
            // TODO: Convert to three-way road
            return null;
        }
        else if (connectedRoads.Count == 1)
        {
            Vector3 otherPos = connectedRoads[0].transform.position;
            if (Vector3.Dot((otherPos - transform.position).normalized, vector.normalized) == 1)
            {
                RoadConnection connection = GetRoadConnectionFromVector(vector);
                connection.connectedTo = other;
                return connection;
            }
            else
            {
                // TODO: Convert to elbow road
                return null;
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-vector, transform.up);
            roadConnections[0].connectedTo = other;
            return roadConnections[0];
        }
    }

    public override void HandleRoadPlacement(RoadPiece toPlace)
    {
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        Vector3 otherPos = toPlace.transform.position;
        if (connectedRoads.Count == 0)
        {
            transform.rotation = Quaternion.LookRotation(otherPos - transform.position, transform.up);
            Vector3 vector = otherPos - transform.position;
            Debug.Log(vector);
            if (vector.z > 0)
            {
                RoadConnection connect = roadConnections[0];
                connect = toPlace.AddConnectionFromVector(vector, connect);
            }
            else if (vector.z < 0)
            {
                RoadConnection connect = roadConnections[1];
                connect = toPlace.AddConnectionFromVector(vector, connect);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(vector, transform.up);
                RoadConnection connect = roadConnections[0];
                connect = toPlace.AddConnectionFromVector(vector, connect);
            }
        }
    }

    // public override PathCreator GetConnectedPath(RoadPiece otherRoad, int laneType)
    // {
    //     switch (_roadTypes[otherRoad.GetType()])
    //     {
    //         case 0: // TwoDirectionRoad
    //             return _paths[laneType];
    //         case 1: // FourWayIntersection
    //             return _paths[]
    //         default:
    //             return null;
    //     }
    // }
}
