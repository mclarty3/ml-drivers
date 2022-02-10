using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public GameObject nodePrefab;
    public Material highlightMaterial;

    public int sizeX;
    public int sizeZ;
    public float spacing;
    public int verticalOffset = 0;
    public float objectOffset = 0.05f;

    public Node[,] grid;

    private static GridBase instance = null;
    public Transform gridHolder;

    public static GridBase GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        CreateGrid();
        CreateMouseCollision();
    }

    void CreateGrid()
    {
        grid = new Node[sizeX, sizeZ];
        gridHolder = new GameObject("Grid").transform;
        gridHolder.parent = transform;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                float posX = x * spacing;
                float posZ = z * spacing;
                GameObject go = Instantiate(nodePrefab, new Vector3(posX, verticalOffset, posZ),
                                              Quaternion.identity) as GameObject;
                go.transform.parent = gridHolder;

                NodeObject nodeObj = go.GetComponent<NodeObject>();
                nodeObj.posX = x;
                nodeObj.posZ = z;

                Node node = new Node();
                node.x = x;
                node.z = z;
                grid[x, z] = node;
            }
        }
    }

    void CreateMouseCollision()
    {
        GameObject go = new GameObject("MouseCollision");
        go.AddComponent<BoxCollider>();
        go.GetComponent<BoxCollider>().size = new Vector3(sizeX * spacing, verticalOffset + 0.01f, 
                                                          sizeZ * spacing);
        go.transform.parent = gridHolder;
        go.transform.position = new Vector3((sizeX / 2) * spacing, verticalOffset, 
                                            (sizeZ / 2) * spacing);
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition, out bool isOnGrid)
    {
        int x = Mathf.FloorToInt((worldPosition.x + (spacing / 2)) / spacing);
        int z = Mathf.FloorToInt((worldPosition.z + (spacing / 2)) / spacing);
        isOnGrid = true;

        if (x < 0 || x >= sizeX)
        {
            x = x < 0 ? 0 : sizeX - 1;
            isOnGrid = false;
        }
        if (z < 0 || z >= sizeZ)
        {
            z = z < 0 ? 0 : sizeZ - 1;
            isOnGrid = false;
        }

        return grid[x, z];
    }

    public Transform GetNodeTransform(Node node)
    {
        int index = node.x * sizeZ + node.z;
        return gridHolder.GetChild(index);
    }

    public Node GetAdjacentNode(Node node, int direction)
    {
        int x = 0;
        int z = 0;
        
        switch (direction)
        {
            case 0:
                x = node.x;
                z = node.z + 1;
                break;
            case 1:
                x = node.x + 1;
                z = node.z + 1;
                break;
            case 2:
                x = node.x + 1;
                z = node.z;
                break;
            case 3:
                x = node.x + 1;
                z = node.z - 1;
                break;
            case 4:
                x = node.x;
                z = node.z - 1;
                break;
            case 5:
                x = node.x - 1;
                z = node.z - 1;
                break;
            case 6:
                x = node.x - 1;
                z = node.z;
                break;
            case 7:
                x = node.x - 1;
                z = node.z + 1;
                break;
        }
        if (x < 0 || x >= sizeX || z < 0 || z >= sizeZ)
        {
            return null;
        }
        
        return grid[x, z];
    }

    public Node[] GetSurroundingNodes(Node node, bool includeCornerNodes=false)
    {
        Node[] surroundingNodes;
        if (includeCornerNodes) {
            surroundingNodes = new Node[8];
        } else {
            surroundingNodes = new Node[4];
        }

        for (int i = 0; i < surroundingNodes.Length; i++)
        {
            surroundingNodes[i] = GetAdjacentNode(node, includeCornerNodes ? i : i * 2);
        }

        return surroundingNodes;

        // List<Node> surroundingNodes = new List<Node>();
        // int i = 0;
        // // Top
        // if (node.z < sizeZ - 1)
        // {
        //     surroundingNodes[i] = grid[node.x, node.z + 1];
        // }
        // i += 1;
        // // Right
        // if (node.x < sizeX - 1)
        // {
        //     surroundingNodes[i] = grid[node.x + 1, node.z];
        // }
        // i += 1;
        // // Bottom
        // if (node.z > 0)
        // {
        //     surroundingNodes[i] = grid[node.x, node.z - 1];
        // }
        // i += 1;
        // // Left
        // if (node.x > 0)
        // {
        //     surroundingNodes[i] = grid[node.x - 1, node.z];
        // }
        return surroundingNodes;
    }

    public void HighlightNode(Node node)
    {
        int index = node.x * sizeZ + node.z;
        Transform nodeObj = GetNodeTransform(node);
        nodeObj.GetComponent<Renderer>().material = highlightMaterial;
    }

    public void UnHighlightNode(Node node)
    {
        int index = node.x * sizeZ + node.z;
        Transform nodeObj = GetNodeTransform(node);
        nodeObj.GetComponent<Renderer>().material = nodePrefab.GetComponent<Renderer>().sharedMaterial;
    }
}
