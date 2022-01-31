using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour
{
    public int posX;
    public int posZ;
    public int textureId;

    public void UpdateNodeObject(Node currentNode)
    {
        // posX = saveable.posX;
        // posZ = saveable.posZ;
        // textureId = saveable.textureId;

        // ChangeMaterial(currentNode);
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

[System.Serializable]
public class NodeObjectSerializable
{
    public int posX;
    public int posZ;
    public int textureId;
}