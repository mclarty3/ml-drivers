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
    private Transform gridHolder;

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

    public void ResetGrid()
    {
        Destroy(gridHolder.gameObject);
        CreateGrid();
        CreateMouseCollision();
        return;
    }

    public void ResetGridFromData(int[] gridData=null, Dictionary<int, GameObject> prefabIds=null)
    {
        int sizeX = gridData[0];
        int sizeZ = gridData[1];
        if (gridData.Length - 2 != sizeX * sizeZ * 2)
        {
            Debug.LogError("prefabIds.Length != sizeX * sizeZ");
            return;
        }

        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        ResetGrid();

        for (int i = 2; i < gridData.Length; i+=2)
        {
            int prefabId = gridData[i];
            if (prefabId == 0)
            {
                continue;
            }

            int x = (i - 2) / (sizeZ * 2);
            int z = (i - 2) / 2 % sizeX;
            Node node = grid[x, z];
            Transform nodeTransform = GetNodeTransform(node);
            int angle = gridData[i + 1];
            node.vis = Instantiate(prefabIds[prefabId], nodeTransform.position + objectOffset * Vector3.up,
                                   Quaternion.Euler(0, angle, 0)) as GameObject;
            node.objId = prefabId;
            node.vis.transform.parent = nodeTransform;
        }
        ReconnectAllRoads();
    }

    void ReconnectAllRoads()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Node node = grid[x, z];
                if (node.vis == null)
                {
                    continue;
                }

                RoadPiece roadPiece = node.vis.GetComponent<RoadPiece>();
                Node[] surroundingNodes = GetSurroundingNodes(node);

                foreach (Node other in surroundingNodes)
                {
                    if (other == null || other.vis == null)
                    {
                        continue;
                    }


                    RoadPiece otherPiece = other.vis.GetComponent<RoadPiece>();
                    Vector3 direction = other.vis.transform.position - node.vis.transform.position;
                    RoadConnection connect = roadPiece.GetRoadConnectionFromVector(-direction);
                    RoadConnection otherConnect = otherPiece.GetRoadConnectionFromVector(direction);

                    connect.ConnectTo(otherConnect);
                    otherConnect.ConnectTo(connect);
                }
            }
        }
    }

    public bool IsDisconnectedRoad()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Node node = grid[x, z];
                if (node.vis == null || !node.vis.TryGetComponent<RoadPiece>(out RoadPiece roadPiece))
                {
                    continue;
                }

                foreach (RoadConnection conn in roadPiece.roadConnections)
                {
                    if (conn.connectedTo == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
