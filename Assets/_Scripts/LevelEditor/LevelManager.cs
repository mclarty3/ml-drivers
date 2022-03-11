using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    GridBase gridBase;
    ModalManager modalManager;
    ModalManager loadMapModalManager;
    InterfaceManager interfaceManager;
    CarSpawner carSpawner;

    public List<GameObject> inSceneGameObjects;
    public List<GameObject> inSceneRoadPieces;

    public Dictionary<string, GameObject> prefabDict;
    public Dictionary<int, GameObject> prefabIdDict;
    public InputField mapName;
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
            },
            {
                "FourWayTrafficLight",
                Resources.Load<GameObject>("Prefabs/TrafficSignals/FourWayTrafficLightManager")
            },
            {
                "FourWayStopSign",
                Resources.Load<GameObject>("Prefabs/TrafficSignals/FourWayStopManager")
            },
            {
                "ThreeWayTrafficLight",
                Resources.Load<GameObject>("Prefabs/TrafficSignals/ThreeWayTrafficLightManager")
            },
            {
                "ThreeWayStopSign",
                Resources.Load<GameObject>("Prefabs/TrafficSignals/ThreeWayStopManager")
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

    // Start is called before the first frame update
    void Start()
    {
        gridBase = GridBase.GetInstance();
        modalManager = GetComponent<ModalManager>();
        interfaceManager = InterfaceManager.GetInstance();
        carSpawner = GetComponent<CarSpawner>();

    }

    public int GetRoadPiecePrefabId(string roadPieceName)
    {
        int id = prefabIdDict.FirstOrDefault(x => roadPieceName.Contains(x.Value.name)).Key;
        Debug.Log(id);
        return id;
    }

    public void SaveLevel(bool skipCheck=false)
    {
        if (String.IsNullOrWhiteSpace(mapName.text))
        {
            return;
        }

        if (gridBase.IsDisconnectedRoad() && !skipCheck)
        {
            modalManager.SetBodyText("You are about to save a level that contains unconnected road " +
                                     "segments. This level cannot be used in the Simulation Space. " +
                                     "Are you sure you want to save this level?");
            modalManager.ClearModalConfirmButtonOnClick();
            modalManager.SetModalConfirmButtonOnClick(() => SaveLevel(true));
            modalManager.SetModalConfirmButtonOnClick(() => modalManager.OpenModal(false));
            modalManager.SetModalConfirmButtonOnClick(() => interfaceManager.MouseExit());
            modalManager.OpenModal();
            return;
        }

        SaveLoadManager.SaveLevel(gridBase, mapName.text, false);
    }

    public void LoadLevel(Text levelName)
    {
        int[] loadedData = SaveLoadManager.LoadLevel(levelName.text, false);

        if (loadedData != null)
        {
            gridBase.ResetGridFromData(loadedData, prefabIdDict);
        }
    }

    public void ClearLevel(bool skipCheck=false)
    {
        if (!skipCheck)
        {
            modalManager.SetBodyText("Are you sure you want to clear the level?");
            modalManager.ClearModalConfirmButtonOnClick();
            modalManager.SetModalConfirmButtonOnClick(() => ClearLevel(true));
            modalManager.SetModalConfirmButtonOnClick(() => modalManager.OpenModal(false));
            modalManager.SetModalConfirmButtonOnClick(() => interfaceManager.MouseExit());
            modalManager.SetModalConfirmButtonOnClick(() => carSpawner.EndSimulation());
            modalManager.OpenModal();
            return;
        }

        gridBase.ResetGrid();
    }
}
