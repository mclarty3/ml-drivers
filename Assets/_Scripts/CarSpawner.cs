﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public int numCarsAtStart = 20;
    public bool simulationActive { get; private set; }
    GridBase grid;
    List<Path> paths;
    List<PathCrawler> crawlers;
    InterfaceManager interfaceManager;

    // Start is called before the first frame update
    void Start()
    {
        grid = GridBase.GetInstance();
        interfaceManager = InterfaceManager.GetInstance();
        crawlers = new List<PathCrawler>();
        simulationActive = false;
    }

    public void BeginSimulation()
    {
        bool disconnectedPath = true;
        InitializePaths(grid.GetRoadPieces(ref disconnectedPath));

        if (paths.Count == 0)
        {
            return;
        }
        else if (disconnectedPath)
        {
            return;
        }

        for (int i = 0; i < numCarsAtStart; i++)
        {
            SpawnCarOnRandomPath();
        }
        simulationActive = true;
        interfaceManager.ToggleSimulationButton(simulationActive);
    }

    public void EndSimulation()
    {
        foreach (PathCrawler crawler in crawlers)
        {
            Destroy(crawler.gameObject);
        }
        crawlers.Clear();
        simulationActive = false;
        interfaceManager.ToggleSimulationButton(simulationActive);
    }

    public void InitializePaths(List<RoadPiece> roadPieces)
    {
        paths = new List<Path>();
        foreach (RoadPiece roadPiece in roadPieces)
        {
            foreach (RoadConnection connection in roadPiece.roadConnections)
            {
                paths.AddRange(connection.outPaths);
                paths.AddRange(connection.inPaths);
            }
        }
    }

    public void SpawnCar(Path pathToSpawn)
    {
        GameObject newCar = Instantiate(carPrefab, pathToSpawn.nodes[0], Quaternion.identity);
        PathCrawler pathCrawler = newCar.GetComponent<PathCrawler>();
        pathCrawler.currentPath = pathToSpawn;
        crawlers.Add(pathCrawler);
    }

    public void SpawnCarOnRandomPath()
    {
        int randomIndex = UnityEngine.Random.Range(0, paths.Count);
        SpawnCar(paths[randomIndex]);
    }
}
