using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    LevelManager manager;
    GridBase gridBase;
    InterfaceManager interfaceManager;

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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        UpdateHighlightedNode();

        if (Input.GetMouseButtonDown(0))
        {
            if (objToPlace != null && !interfaceManager.mouseOverUIElement)
            {
                CheckPlaceObject();
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
        Node node = gridBase.NodeFromWorldPosition(mousePosition);
        if (node != highlightedNode)
        {
            if (highlightedNode != null)
            {
                // gridBase.UnHighlightNode(highlightedNode);
            }
            highlightedNode = node;
            // gridBase.HighlightNode(highlightedNode);
            if (objToPlace && highlightedNode.vis == null)
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

    void CheckPlaceObject()
    {
        // if (objToPlace != null && highlightedNode.vis == null)
        // {
        //     RoadPiece roadToPlace = objHighlight.GetComponent<RoadPiece>();
        //     Transform toPlaceTransform = gridBase.GetNodeTransform(highlightedNode);
        //     Node[] surroundingNodes = gridBase.GetSurroundingNodes(highlightedNode);
        //     int numSurroundingRoads = 0;
        //     foreach (Node node in surroundingNodes)
        //     {
        //         if (node.vis != null)
        //         {
        //             numSurroundingRoads++;
        //         }
        //     }
        //     foreach (Node node in surroundingNodes)
        //     {
        //         if (node.vis != null)
        //         {
        //             node.vis.GetComponent<RoadPiece>().HandleRoadPlacement(roadToPlace);
        //         }
        //     }
        //     // if (numSurroundingRoads == 1)
        //     // {
        //     //     foreach (Node node in surroundingNodes)
        //     //     {
        //     //         if (node.vis != null)
        //     //         {
        //     //             Transform otherTrans = gridBase.GetNodeTransform(node);
        //     //             if ((otherTrans.position - toPlaceTransform.position).z != 0)
        //     //             {
        //     //                 Quaternion rot = Quaternion.LookRotation(gridBase.gridHolder.forward,
        //     //                                                          gridBase.gridHolder.up);
        //     //                 objHighlight.transform.rotation = rot;
        //     //                 node.vis.transform.rotation = rot;
        //     //             }
        //     //             else
        //     //             {
        //     //                 Quaternion rot = Quaternion.LookRotation(gridBase.gridHolder.right,
        //     //                                                          gridBase.gridHolder.up);
        //     //                 objHighlight.transform.rotation = rot;
        //     //                 node.vis.transform.rotation = rot;
        //     //             }
        //     //         }
        //     //     }
        //     // }
        // }
    }

    public void PlaceObject()
    {
        if (objToPlace != null && highlightedNode != null)
        {
            Transform nodeTransform = gridBase.GetNodeTransform(highlightedNode);
            Vector3 pos = nodeTransform.position;
            pos.y = pos.y + gridBase.objectOffset;
            GameObject obj = Instantiate(objToPlace, pos, objHighlight.transform.rotation);
            obj.transform.parent = nodeTransform;
            highlightedNode.vis = obj;
            manager.inSceneGameObjects.Add(obj);
            if (objHighlight)
            {
                Destroy(objHighlight);
            }

            RoadPiece road = obj.GetComponent<RoadPiece>();
            Node[] surroundingNodes = gridBase.GetSurroundingNodes(highlightedNode);
            int numSurroundingRoads = 0;
            foreach (Node node in surroundingNodes)
            {
                if (node.vis != null)
                {
                    numSurroundingRoads++;
                }
            }
            foreach (Node node in surroundingNodes)
            {
                if (node.vis != null)
                {
                    // Get changed objects (surroundingRoad, placedRoad)
                    GameObject[] newVis = node.vis.GetComponent<RoadPiece>().HandleRoadPlacement(road);

                    if (newVis[0] != null)
                    {
                        node.vis = newVis[0];
                        newVis[0].transform.parent = gridBase.GetNodeTransform(node);
                    }
                    if (newVis[1] != null)
                    {
                        road = newVis[1].GetComponent<RoadPiece>();
                        highlightedNode.vis = newVis[1];
                        newVis[1].transform.parent = gridBase.GetNodeTransform(highlightedNode);
                    }
                }
            }
        }
    }

    public void SetObjectToPlace(GameObject obj)
    {
        objToPlace = obj;
    }
}
