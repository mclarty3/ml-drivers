using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowPath : MonoBehaviour
{
    PathCreator creator;
    Vector3[] nodes;

    // Start is called before the first frame update
    void Start()
    {
        creator = GetComponent<PathCreator>();
    }

    void GeneratePath(float spacing, float resolution)
    {
        return;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}