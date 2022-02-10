using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public abstract class RoadPiece : MonoBehaviour
{
    [SerializeField]
    public List<RoadConnection> roadConnections = new List<RoadConnection>();

    protected Dictionary<Type, int> _roadTypes = new Dictionary<Type, int>
    {
        { typeof(TwoDirectionRoad), 0 },
        { typeof(FourWayIntersection), 1 },
    };

    public List<RoadConnection> GetConnectedRoads()
    {
        List<RoadConnection> connectedRoads = new List<RoadConnection>();
        foreach (RoadConnection connection in roadConnections)
        {
            if (connection.connectedTo != null)
            {
                connectedRoads.Add(connection);
            }
        }

        return connectedRoads;
    }

    protected abstract RoadConnection GetRoadConnectionFromVector(Vector3 vector);
    public abstract RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go);

    public abstract GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false);
}
