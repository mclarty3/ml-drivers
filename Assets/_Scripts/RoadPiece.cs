using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public abstract class RoadPiece : MonoBehaviour
{
    // [Serializable]
    // public struct RoadConnection
    // {
    //     [SerializeField]
    //     public List<Path> outPaths;
    //     [SerializeField]
    //     public List<Path> inPaths;
    //     public RoadConnectionCollider connectionCollider;
    // }

    [SerializeField]
    protected Path[] _paths;
    private List<RoadPiece> connectedRoads;
    [SerializeField]
    public List<RoadConnection> roadConnections = new List<RoadConnection>();

    protected Dictionary<Type, int> _roadTypes = new Dictionary<Type, int>
    {
        { typeof(TwoDirectionRoad), 0 },
        { typeof(FourWayIntersection), 1 },
    };

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
}
