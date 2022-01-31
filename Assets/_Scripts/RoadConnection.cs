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
    public RoadConnection connectedTo;

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
        
    }

    void ConnectTo(RoadConnection other)
    {
        connectedTo = other;
        foreach (Path path in outPaths)
        {
            path.connectingPaths = other.inPaths;
        }
    }
}
