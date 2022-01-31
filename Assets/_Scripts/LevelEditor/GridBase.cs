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

                NodeObject nodeObj = go.GetComponent<NodeObject>();
                nodeObj.posX = x;
                nodeObj.posZ = z;

                Node node = new Node();
                // node.vis = go;
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

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x + (spacing / 2)) / spacing);
        int z = Mathf.FloorToInt((worldPosition.z + (spacing / 2)) / spacing);

        if (x >= sizeX) 
        {
            x = sizeX - 1;
        }
        if (z >= sizeZ) 
        {
            z = sizeZ - 1;
        }
        if (x < 0) x = 0;
        if (z < 0) z = 0;

        return grid[x, z];
    }

    public Transform GetNodeTransform(Node node)
    {
        int index = node.x * sizeZ + node.z;
        return gridHolder.GetChild(index);
    }

    public List<Node> GetSurroundingNodes(Node node)
    {
        List<Node> surroundingNodes = new List<Node>();
        if (node.x > 0)
        {
            surroundingNodes.Add(grid[node.x - 1, node.z]);
        }
        if (node.x < sizeX - 1)
        {
            surroundingNodes.Add(grid[node.x + 1, node.z]);
        }
        if (node.z > 0)
        {
            surroundingNodes.Add(grid[node.x, node.z - 1]);
        }
        if (node.z < sizeZ - 1)
        {
            surroundingNodes.Add(grid[node.x, node.z + 1]);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
