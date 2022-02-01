﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Path : MonoBehaviour
{
    [SerializeField]
    public PathCreator path;
    public List<Path> connectingPaths = new List<Path>();
    private Vector3[] _nodes = null;
    public Vector3[] nodes { 
        get {
            if (_nodes == null) {
                _nodes = GetPathNodes(path);
            }
            return _nodes;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (path == null) {
            path = GetComponent<PathCreator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Path GetConnectingPath(List<Path> paths)
    {
        if (paths.Count == 0) {
            return null;
        } else if (paths.Count == 1) {
            return paths[0];
        } else {
            return paths[Random.Range(0, paths.Count)];
        }
    }

    public Path GetConnectingPath()
    {
        return Path.GetConnectingPath(connectingPaths);
    }

    public Vector3[] GetPathNodes(PathCreator path)
    {
        Vector3[] nodes = new Vector3[path.path.NumPoints];
        for (int i = 0; i < path.path.NumPoints; i++)
        {
            nodes[i] = path.path.GetPoint(i);
        }
        return nodes;
    }
}