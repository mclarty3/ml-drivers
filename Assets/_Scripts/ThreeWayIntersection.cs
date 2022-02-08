using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeWayIntersection : RoadPiece
{
    public static GameObject prefab = null;
    // Start is called before the first frame update
    void Start()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/ThreeWayIntersection");
        }

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

        if (prefab == null)
        {
            Debug.LogError("ThreeWayIntersection must have a prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected override RoadConnection GetRoadConnectionFromVector(Vector3 vector)
    {
        if (vector == Vector3.forward)
        {
            return roadConnections[0];
        }
        else if (vector == Vector3.right)
        {
            return roadConnections[1];
        }
        else if (vector == Vector3.back)
        {
            return roadConnections[2];
        }
        else if (vector == Vector3.left)
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
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        if (connectedRoads.Count == 3)
        {

        }
        go = null;
        return null;
    }

    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false)
    {
        List<RoadConnection> connectedRoads = GetConnectedRoads();
        List<RoadConnection> placementConnections = toPlace.GetConnectedRoads();

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

    public GameObject ConvertToFourWay(RoadPiece newPiece)
    {
        Vector3 toNewPiece = newPiece.transform.position - transform.position;
        GameObject newRoad = Instantiate(FourWayIntersection.prefab, transform.position, 
                                         transform.rotation);

        FourWayIntersection fourWay = newRoad.GetComponent<FourWayIntersection>();
        RoadConnection newConnect = newPiece.AddConnectionFromVector(toNewPiece, 
                                                                     fourWay.roadConnections[3], 
                                                                     this, out GameObject go);
        fourWay.roadConnections[3].ConnectTo(newConnect);
        for (int i = 0; i < 3; i++)
        {
            fourWay.roadConnections[i].ConnectTo(roadConnections[i].connectedTo);
            roadConnections[i].connectedTo.ConnectTo(fourWay.roadConnections[i]);
        }
        Destroy(gameObject);
        return newRoad;
    }
}
