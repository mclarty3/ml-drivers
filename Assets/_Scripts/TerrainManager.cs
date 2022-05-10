using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    Terrain terrain;

    public int treeDistanceOverride = 5000;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.treeDistance = treeDistanceOverride;
    }

    // Update is called once per frame
    void Update()
    {
        if (treeDistanceOverride != terrain.treeDistance) {
            terrain.treeDistance = treeDistanceOverride;
        }
    }
}
