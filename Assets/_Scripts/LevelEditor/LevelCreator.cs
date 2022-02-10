using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    LevelManager manager;
    GridBase gridBase;
    InterfaceManager interfaceManager;
    DebugLogger log;

    GameObject objToPlace = null;
    GameObject objHighlight = null;
    public Vector3 mousePosition;
    Vector3 worldPosition;

    public Node highlightedNode = null;

    // Start is called before the first frame update
    void Start()
    {
        gridBase = GridBase.GetInstance();
        manager = LevelManager.GetInstance();
        interfaceManager = InterfaceManager.GetInstance();
        log = DebugLogger.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        UpdateHighlightedNode();

        if (Input.GetMouseButton(0))
        {
            if (objToPlace != null && !interfaceManager.mouseOverUIElement)
            {
                PlaceObject();
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
    }

    void UpdateHighlightedNode()
    {
        Node node = gridBase.NodeFromWorldPosition(mousePosition, out bool isOnGrid);
        if (node != highlightedNode)
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
            else if (objHighlight != null && highlightedNode.vis != null)
            {
                objHighlight.SetActive(true);
            } 
            else if (objHighlight != null)
            {
                Destroy(objHighlight);
                objHighlight = null;
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
        int numSurroundingRoads = 0;
        bool invalidPlacement = false;
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

        log.Log("Placing object - checking surrounding roads");

        foreach (Node node in surroundingNodes)
        {
            if (node != null && node.vis != null)
            {
                numSurroundingRoads++;
            }
        }
        for (int i = 0; i < surroundingNodes.Length; i+=2)
        {
            Node node = surroundingNodes[i];
            if (node != null && node.vis != null)
            {
                // Get changed objects (surroundingRoad, placedRoad)
                GameObject[] newVis = node.vis.GetComponent<RoadPiece>().HandleRoadPlacement(road);

                if (newVis[0] != null)
                {
                    log.Log("Converting surrounding road piece");
                    node.vis = newVis[0];
                    newVis[0].transform.parent = gridBase.GetNodeTransform(node);
                }
                if (newVis[1] != null)
                {
                    log.Log("Converting placed road piece");
                    road = newVis[1].GetComponent<RoadPiece>();
                    highlightedNode.vis = newVis[1];
                    newVis[1].transform.parent = gridBase.GetNodeTransform(highlightedNode);
                }
            }
        }
    
        obj.transform.parent = nodeTransform;
        highlightedNode.vis = obj;
        manager.inSceneGameObjects.Add(obj);
        if (objHighlight)
        {
            Destroy(objHighlight);
        }
    }

    public void SetObjectToPlace(GameObject obj)
    {
        objToPlace = obj;
    }
}
