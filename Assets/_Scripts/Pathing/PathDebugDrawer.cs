using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDebugDrawer : MonoBehaviour
{
    Path[] paths;

    // Start is called before the first frame update
    void Start()
    {
        paths = Transform.FindObjectsOfType<Path>() as Path[];
    }

    // Update is called once per frame
    void Update()
    {
        Path[] scenePaths = Transform.FindObjectsOfType<Path>() as Path[];
        if (paths.Length != scenePaths.Length) {
            paths = scenePaths;
        }
    }

    void DrawPaths()
    {
        foreach (Path path in paths)
        {
            // List<Vector3> bezierNodes = path.path.bezierPath.po
            Vector3[] nodes = path.nodes;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Vector3 offset = new Vector3(0, 0.5f, 0);
                Debug.DrawLine(nodes[i] + offset, nodes[i + 1] + offset, Color.red);
                Gizmos.DrawSphere(nodes[i] + offset, 0.1f);
            }
        }
    }

    void OnDrawGizmos()
    {
        DrawPaths();
    }
}
