using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    GridBase gridBase;

    public List<GameObject> inSceneGameObjects;
    public List<GameObject> inSceneRoadPieces;

    public Dictionary<string, GameObject> prefabDict;
    public Dictionary<int, GameObject> prefabIdDict;
    public bool printDebugMessages = false;

    private static LevelManager _instance;
    public static LevelManager GetInstance()
    {
        return _instance;
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gridBase = GridBase.GetInstance();

        prefabDict = new Dictionary<string, GameObject>()
        {
            {
                "TwoDirectionRoad",
                Resources.Load<GameObject>("Prefabs/RoadPieces/TwoDirectionRoad")
            },
            {
                "ElbowRoad",
                Resources.Load<GameObject>("Prefabs/RoadPieces/ElbowRoad")
            },
            {
                "ThreeWayIntersection",
                Resources.Load<GameObject>("Prefabs/RoadPieces/ThreeWayIntersection")
            },
            {
                "FourWayIntersection",
                Resources.Load<GameObject>("Prefabs/RoadPieces/FourWayIntersection")
            }
        };

        prefabIdDict = new Dictionary<int, GameObject>();
        int i = 1;
        foreach (KeyValuePair<string, GameObject> entry in prefabDict)
        {
            prefabIdDict.Add(i, entry.Value);
            i += 1;
        }
    }

    public int GetRoadPiecePrefabId(string roadPieceName)
    {
        int id = prefabIdDict.FirstOrDefault(x => roadPieceName.Contains(x.Value.name)).Key;
        return id;
    }

    public void SaveLevel()
    {
        SaveLoadManager.SaveLevel(gridBase);
    }

    public void LoadLevel()
    {
        int[] loadedData = SaveLoadManager.LoadLevel();

        if (loadedData != null)
        {
            gridBase.ResetGrid(loadedData, prefabIdDict);
        }
    }
}
