using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GridBase gridBase;

    public List<GameObject> inSceneGameObjects;
    public List<GameObject> inSceneRoadPieces;

    public Dictionary<string, GameObject> prefabDict;

    private static LevelManager instance = null;
    public static LevelManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        prefabDict = new Dictionary<string, GameObject>()
        {
            { "TwoDirectionRoad",       Resources.Load<GameObject>("Prefabs/TwoDirectionRoad") },
            { "ElbowRoad",              Resources.Load<GameObject>("Prefabs/ElbowRoad") },
            { "ThreeWayIntersection",   Resources.Load<GameObject>("Prefabs/ThreeWayIntersection") },
            { "FourWayIntersection",    Resources.Load<GameObject>("Prefabs/FourWayIntersection") }
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        gridBase = GridBase.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
