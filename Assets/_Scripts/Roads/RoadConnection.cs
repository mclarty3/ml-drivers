using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoadConnection : MonoBehaviour
{
    public RoadPiece roadPiece = null;
    public List<Path> outPaths;
    public List<Path> inPaths;
    public RoadConnection connectedTo = null;
    public TrafficLightGroup trafficLightGroup = null;
    public int lightColor = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (roadPiece == null)
        {
            roadPiece = GetComponentInParent<RoadPiece>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (trafficLightGroup != null && trafficLightGroup.trafficLights.Length > 0) {
            lightColor = trafficLightGroup.lightColour;
        }
    }

    public void ConnectTo(RoadConnection other)
    {
        connectedTo = other;
        foreach (Path path in outPaths)
        {
            // path.connectingPaths = other.inPaths;
            path.connectingPaths = other == null ? null : other.inPaths;
            if (other.trafficLightGroup != null) {
                path.connectedTrafficLight = other.trafficLightGroup;
            }
        }
    }
}
