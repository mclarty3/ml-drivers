using System.Collections;
using System.Collections.Generic;
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
            if (objToPlace)
            {
                Vector3 highlightPos = gridBase.GetNodeTransform(node).position;
                highlightPos.y = highlightPos.y + 0.05f;
                if (objHighlight == null)
                {
                    Debug.Log("Instantiating highlight");
                    objHighlight = Instantiate(objToPlace, highlightPos, Quaternion.identity);
                }
                else
                {
                    objHighlight.transform.position = highlightPos;
                }
            } else if (objHighlight != null)
            {
                Destroy(objHighlight);
            }
        }
    }

    public void PlaceObject()
    {
        List<Node> surroundingNodes = gridBase.GetSurroundingNodes(highlightedNode);
        foreach (Node node in surroundingNodes)
        {
            if (node.vis != null)
                Debug.Log(node.vis.name);
        }
        if (objToPlace != null && highlightedNode != null)
        {
            Transform nodeTransform = gridBase.GetNodeTransform(highlightedNode);
            Vector3 pos = nodeTransform.position;
            pos.y = pos.y + gridBase.objectOffset;
            GameObject obj = Instantiate(objToPlace, pos, Quaternion.identity);
            obj.transform.parent = nodeTransform;
            highlightedNode.vis = obj;
            manager.inSceneGameObjects.Add(obj);
            if (objHighlight)
            {
                Destroy(objHighlight);
            }
        }
    }

    public void SetObjectToPlace(GameObject obj)
    {
        objToPlace = obj;
    }
}
