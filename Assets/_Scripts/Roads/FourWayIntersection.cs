using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class FourWayIntersection : RoadPiece
{
    TrafficLightManager tlm;

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
        AddTrafficLight();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddTrafficLight()
    {
        GameObject prefab = LevelManager.GetInstance().prefabDict["FourWayTrafficLight"];
        GameObject trafficLight = Instantiate(prefab, transform.position, transform.rotation);
        trafficLight.transform.parent = transform;
        tlm = trafficLight.GetComponent<TrafficLightManager>();
        Debug.Log(tlm.trafficLightGroups.Length);

        roadConnections[1].trafficLightGroup = tlm.trafficLightGroups[0];
        roadConnections[2].trafficLightGroup = tlm.trafficLightGroups[0];
        roadConnections[0].trafficLightGroup = tlm.trafficLightGroups[1];
        roadConnections[3].trafficLightGroup = tlm.trafficLightGroups[1];

        foreach (RoadConnection roadConnection in roadConnections) {
            if (roadConnection.connectedTo == null) continue;

            foreach (Path path in roadConnection.connectedTo.outPaths) {
                path.connectedTrafficLight = roadConnection.trafficLightGroup;
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
