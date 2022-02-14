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

    public List<RoadConnection> GetFullConnections()
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

    public List<RoadConnection> GetConnectedRoads()
    {
        List<RoadConnection> connectedRoads = new List<RoadConnection>();
        foreach (RoadConnection connection in roadConnections)
        {
            if (connection.connectedTo != null)
            {
                connectedRoads.Add(connection.connectedTo);
            }
        }

        return connectedRoads;
    }

    /// <summary>
    /// Returns the connection that faces the direction a given vector comes from.
    /// </summary>
    protected abstract RoadConnection GetRoadConnectionFromVector(Vector3 vector);

    /// <summary>
    /// Returns a road connection from this road, and adds an additional connector to the road
    /// if applicable (TwoWay -> ThreeWay, ThreeWay -> FourWay)
    /// </summary>
    public abstract RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go);

    /// <summary>
    /// Given another RoadPiece placed adjacent to this road piece, connect the given roads and convert
    /// one or both of them to a different road type if necessary.
    /// </summary>
    public abstract GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false);

    public abstract GameObject HandleRoadRemoval(RoadPiece toRemove);
}
