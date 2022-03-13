using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoadConnection : MonoBehaviour
{
    public RoadPiece roadPiece = null;
    public List<NodePath> outPaths;
    public List<NodePath> inPaths;
    public RoadConnection connectedTo = null;
    public TrafficSignalGroup trafficSignalGroup = null;
    public int signalType = -1;

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
        if (trafficSignalGroup != null && trafficSignalGroup.signalType != signalType) {
            signalType = trafficSignalGroup.signalType;
        }
    }

    public void ConnectTo(RoadConnection other)
    {
        connectedTo = other;
        foreach (NodePath path in outPaths)
        {
            // path.connectingPaths = other.inPaths;
            path.connectingPaths = other == null ? null : other.inPaths;
            if (other.trafficSignalGroup != null) {
                path.connectedTrafficSignal = other.trafficSignalGroup;
            }
        }
    }
}
