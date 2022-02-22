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
    public abstract RoadConnection GetRoadConnectionFromVector(Vector3 vector);

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

    /// <summary>
    /// Given an adjacent RoadPiece to be removed, remove the connection from this RoadPiece and convert
    /// to a different road type (FourWay -> ThreeWay, ThreeWay -> Elbow/TwoWay)
    /// </summary>
    public GameObject HandleRoadRemoval(RoadPiece toRemove)
    {
        List<RoadConnection> connectedRoads = GetFullConnections();
        List<RoadConnection> keepConnects = connectedRoads.FindAll(x => x.connectedTo.roadPiece != toRemove);

        // Create a TwoWay road and add connections for each connection that has *not* been removed
        GameObject straightPrefab = LevelManager.GetInstance().prefabDict["TwoDirectionRoad"];
        GameObject newRoad = Instantiate(straightPrefab, transform.position, transform.rotation);
        RoadPiece newPiece = newRoad.GetComponent<RoadPiece>();

        foreach (RoadConnection connection in keepConnects)
        {
            RoadConnection otherConnection = connection.connectedTo;
            otherConnection.connectedTo = null;
            RoadPiece otherPiece = otherConnection.roadPiece;
            Vector3 positionDiff = transform.position - otherPiece.transform.position;
            RoadConnection newConnect = newPiece.AddConnectionFromVector(positionDiff, otherConnection,
                                                                         otherPiece, out GameObject go);
            if (go != null)
            {
                newRoad = go;
                newPiece = newRoad.GetComponent<RoadPiece>();
            }
        }

        Destroy(gameObject);
        return newRoad;
    }
}
