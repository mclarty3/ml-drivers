using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class FourWayIntersection : RoadPiece
{
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
    }

    // Update is called once per frame
    void Update()
    {

    }


    protected override RoadConnection GetRoadConnectionFromVector(Vector3 vector) { return null; }
    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go)
    {
        go = null;
        return null;
    }

    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false)
    { return null; }

    public override GameObject HandleRoadRemoval(RoadPiece toRemove)
    {
        throw new NotImplementedException();
    }
}
