using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
    public string objId;
    public int posX;
    public int posZ;
    public GameObject obj;
    public Vector3 worldPositionOffset;
    public Vector3 worldRotation;

    public float rotateDegrees = 90;

    public void UpdateNode(Node[,] grid)
    {
        Node node = grid[posX, posZ];
        
        Vector3 worldPosition = node.vis.transform.position;
        worldPosition += worldPositionOffset;
        transform.rotation = Quaternion.Euler(worldRotation);
        transform.position = worldPosition;
    }

    public void ChangeRotation()
    {
        Vector3 eulerANgles = transform.eulerAngles;
        eulerANgles += new Vector3(0, rotateDegrees, 0);
        transform.localRotation = Quaternion.Euler(eulerANgles);
    }

    // public SaveableLevelObject GetSaveableObject()
    // {
    //     SaveableLevelObject savedObj = new SaveableLevelObject();
    //     savedObj.objId = objId;
    //     savedObj.posX = posX;
    //     savedObj.posZ = posZ;

    //     worldRotation = transform.localEulerAngles;

    //     savedObj.rotX = worldRotation.x;
    // }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
