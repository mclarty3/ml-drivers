using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    LevelManager manager;
    GridBase gridBase;
    InterfaceManager interfaceManager;
    ModalManager modalManager;
    CarSpawner carSpawner;
    DebugLogger log;

    public GameObject sceneObjectsParent;

    GameObject objToPlace = null;
    GameObject objHighlight = null;
    [HideInInspector]
    public Vector3 mousePosition;
    Vector3 worldPosition;

    public Node highlightedNode = null;

    private bool _placedIntersection = false;

    void Start()
    {
        gridBase = GridBase.GetInstance();
        manager = LevelManager.GetInstance();
        interfaceManager = InterfaceManager.GetInstance();
        modalManager = GetComponent<ModalManager>();
        carSpawner = GetComponent<CarSpawner>();
        log = DebugLogger.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        UpdateHighlightedNode();

        if (Input.GetMouseButton(0))
        {
            if (carSpawner.simulationActive)
            {

            }
            else if (objToPlace != null && !interfaceManager.mouseOverUIElement
                     && highlightedNode != null && highlightedNode.vis == null)
            {
                PlaceObject();
            }
            else if (highlightedNode != null && highlightedNode.vis != null && !_placedIntersection)
            {
                ThreeWayIntersection threeWayIntersection;
                FourWayIntersection fourWayIntersection;
                if (highlightedNode.vis.TryGetComponent<FourWayIntersection>(out fourWayIntersection))
                {
                    fourWayIntersection.CycleTrafficSignal();
                    _placedIntersection = true;
                }
                else if (highlightedNode.vis.TryGetComponent<ThreeWayIntersection>(out threeWayIntersection))
                {
                    threeWayIntersection.CycleTrafficSignal();
                    _placedIntersection = true;
                }
            }
        }
        else
        {
            _placedIntersection = false;
        }

        if (Input.GetMouseButton(1))
        {
            if (carSpawner.simulationActive)
            {

            }
            else if (highlightedNode.vis != null && !interfaceManager.mouseOverUIElement)
            {
                RemoveObject();
            }
        }
    }

    void UpdateMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            mousePosition = hit.point;
        }
        else
        {
            mousePosition = Vector3.positiveInfinity;
        }
    }

    void UpdateHighlightedNode()
    {
        Node node = gridBase.NodeFromWorldPosition(mousePosition, out bool isOnGrid);
        if (node != highlightedNode && !interfaceManager.mouseOverUIElement && !modalManager.isOpen)
        {
            if (highlightedNode != null)
            {
                // gridBase.UnHighlightNode(highlightedNode);
            }
            highlightedNode = node;
            // gridBase.HighlightNode(highlightedNode);

            if (objToPlace && highlightedNode.vis == null && isOnGrid)
            {
                Vector3 highlightPos = gridBase.GetNodeTransform(node).position;
                highlightPos.y = highlightPos.y + 0.05f;
                if (objHighlight == null)
                {
                    objHighlight = Instantiate(objToPlace, highlightPos, Quaternion.identity);
                }
                else
                {
                    objHighlight.transform.position = highlightPos;
                    if (!objHighlight.activeSelf)
                    {
                        objHighlight.SetActive(true);
                    }
                }
            }
            // else if (objHighlight != null && highlightedNode.vis != null)
            // {
            //     objHighlight.SetActive(true);
            // }
            // else if (objHighlight != null)
            else
            {
                Destroy(objHighlight);
                objHighlight = null;
            }
        }
        else if (node != highlightedNode)
        {
            if (objHighlight != null)
            {
                objHighlight.SetActive(false);
            }
        }
    }

    public void PlaceObject()
    {
        if (objToPlace == null || highlightedNode == null || objHighlight == null ||
            highlightedNode.vis != null)
        {
            return;
        }

        log.Log("Attempting to place object at position: (" +
                highlightedNode.x + ", " + highlightedNode.z + ")");

        Transform nodeTransform = gridBase.GetNodeTransform(highlightedNode);
        Vector3 pos = nodeTransform.position + new Vector3(0, gridBase.objectOffset, 0);
        GameObject obj = Instantiate(objToPlace, pos, objHighlight.transform.rotation);

        RoadPiece road = obj.GetComponent<RoadPiece>();
        Node[] surroundingNodes = gridBase.GetSurroundingNodes(highlightedNode, true);
        bool invalidPlacement = false;

        // Check to see if the placement is invalid (road completes a square of four roads)
        for (int i = 0; i < surroundingNodes.Length; i+=2)
        {
            if (surroundingNodes[i] == null || surroundingNodes[i].vis == null)
            {
                continue;
            }
            for (int j = 1; j < 3; j++)
            {
                if (surroundingNodes[(i + j) % 8] == null || surroundingNodes[(i + j) % 8].vis == null)
                {
                    break;
                }
                if (j == 2)
                {
                    invalidPlacement = true;
                    break;
                }
            }
            if (invalidPlacement)
            {
                break;
            }
        }

        if (invalidPlacement)
        {
            Destroy(obj);
            return;
        }

        int count = 0;
        foreach (Node n in surroundingNodes)
        {
            if (n == null || n.vis == null)
            {
                continue;
            }
            count++;
        }

        log.Log("Placing object - checking surrounding roads");
        log.Log("Surrounding roads: " + count);

        for (int i = 0; i < surroundingNodes.Length; i+=2)
        {
            Node node = surroundingNodes[i];
            if (node != null && node.vis != null)
            {
                // Get changed objects (surroundingRoad, placedRoad)
                GameObject[] newVis = node.vis.GetComponent<RoadPiece>().HandleRoadPlacement(road);

				if (newVis.Length == 0)
                {
                    continue;
                }
                if (newVis[0] != null)
                {
                    log.Log("Converting surrounding road piece");
                    node.vis = newVis[0];
                    node.objId = manager.GetRoadPiecePrefabId(node.vis.name);
                    newVis[0].transform.parent = sceneObjectsParent.transform;
                }
                if (newVis[1] != null)
                {
                    log.Log("Converting placed road piece");
                    road = newVis[1].GetComponent<RoadPiece>();
                    highlightedNode.vis = newVis[1];
                    highlightedNode.objId = manager.GetRoadPiecePrefabId(highlightedNode.vis.name);
                    newVis[1].transform.parent = sceneObjectsParent.transform;
                }
            }
        }

        obj.transform.parent = sceneObjectsParent.transform;
        if (highlightedNode.vis == null)
        {
            highlightedNode.vis = obj;
            highlightedNode.objId = manager.GetRoadPiecePrefabId(highlightedNode.vis.name);
        }
        manager.inSceneGameObjects.Add(obj);
        if (objHighlight)
        {
            Destroy(objHighlight);
        }
    }

    void RemoveObject()
    {
        GameObject objToRemove = highlightedNode.vis;
        RoadPiece road = objToRemove.GetComponent<RoadPiece>();
        Node[] surroundingNodes = gridBase.GetSurroundingNodes(highlightedNode);

        foreach (Node node in surroundingNodes)
        {
            if (node == null || node.vis == null)
            {
                continue;
            }
            RoadPiece roadPiece = node.vis.GetComponent<RoadPiece>();
            if (roadPiece != null)
            {
                GameObject newObj = roadPiece.HandleRoadRemoval(road);
                if (newObj != null)
                {
                    node.vis = newObj;
                    node.objId = manager.GetRoadPiecePrefabId(node.vis.name);
                    newObj.transform.parent = sceneObjectsParent.transform;
                }
            }
        }

        highlightedNode.vis = null;
        highlightedNode.objId = 0;
        Destroy(objToRemove);
    }

    public void SetObjectToPlace(GameObject obj)
    {
        objToPlace = obj;
    }
}
