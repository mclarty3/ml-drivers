using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public int numCarsAtStart = 20;
    public float heightOffset = 0.4f;
    public bool simulationActive { get; private set; }
    public bool spawnOnStart = false;
    public GameObject roadParent = null;
    private int _maxSpeed = 15;
    GridBase grid;
    List<NodePath> paths;
    List<PathCrawler> crawlers;
    List<int> occupiedPaths;
    InterfaceManager interfaceManager;

    // Start is called before the first frame update
    void Start()
    {
        grid = GridBase.GetInstance();
        interfaceManager = InterfaceManager.GetInstance();
        crawlers = new List<PathCrawler>();
        occupiedPaths = new List<int>();
        simulationActive = false;

        if (spawnOnStart)
        {
            InitializePaths(new List<RoadPiece>(roadParent.transform.GetComponentsInChildren<RoadPiece>()));
            SpawnCars(numCarsAtStart);
        }
    }

    public void SetNumCars(int numCars)
    {
        if (simulationActive)
        {
            if (numCars < crawlers.Count)
            {
                for (int i = numCars; i < crawlers.Count; i++)
                {
                    PathCrawler crawler = crawlers[0];
                    crawlers.RemoveAt(0);
                    Destroy(crawler.gameObject);
                }
            } else {
                numCarsAtStart = numCars;
                foreach (PathCrawler crawler in crawlers)
                {
                    Destroy(crawler.gameObject);
                }
                crawlers.Clear();
                SpawnCars(numCars);
            }
        }
    }

    public void SetMaxSpeed(int maxSpeed)
    {
        _maxSpeed = maxSpeed;

        if (simulationActive)
        {
            foreach (PathCrawler crawler in crawlers)
            {
                crawler.maxVelocity = _maxSpeed;
            }
        }
    }

    public void BeginSimulation()
    {
        bool disconnectedPath = true;
        if (roadParent != null)
        {
            InitializePaths(new List<RoadPiece>(roadParent.transform.GetComponentsInChildren<RoadPiece>()));
        }
        else
        {
            InitializePaths(grid.GetRoadPieces(ref disconnectedPath));
        }

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
        paths = new List<NodePath>();
        foreach (RoadPiece roadPiece in roadPieces)
        {
            foreach (RoadConnection connection in roadPiece.roadConnections)
            {
                paths.AddRange(connection.outPaths);
                paths.AddRange(connection.inPaths);
            }
        }
    }

    public void SpawnCars(int numCars)
    {
        for (int i = 0; i < numCars; i++)
        {
            SpawnCarOnRandomPath();
        }
    }

    public void SpawnCar(NodePath pathToSpawn)
    {
        Vector3 spawnPos = pathToSpawn.nodes[0] + Vector3.up * heightOffset;
        GameObject newCar = Instantiate(carPrefab, spawnPos, Quaternion.identity);

        Material newMat = Resources.Load("Materials/CarBodyMaterial") as Material;
        // Change albedo color of car body to random color
        newMat.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        foreach (MeshRenderer renderer in newCar.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.name == "Classic_16_Body (Instance)")
                {
                    mat.color = newMat.color;
                }
            }
        }

        PathCrawler pathCrawler = newCar.GetComponent<PathCrawler>();
        pathCrawler.maxVelocity = _maxSpeed;
        // pathCrawler.currentPath = pathToSpawn;
        pathCrawler.Initialize(pathToSpawn);
        Vector3 toNode = pathToSpawn.nodes[1] - pathToSpawn.nodes[0];
        pathCrawler.transform.rotation = Quaternion.LookRotation(toNode);
        crawlers.Add(pathCrawler);
    }

    public void SpawnCarOnRandomPath()
    {
        int randomIndex;
        bool tryNewPath = false;
        do
        {
            tryNewPath = false;
            randomIndex = UnityEngine.Random.Range(0, paths.Count);
            NodePath path = paths[randomIndex];
            Vector3 nodePos = path.nodes[0] + Vector3.up * heightOffset;
            Collider[] colliders = Physics.OverlapSphere(nodePos, 1f);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.tag == "Car")
                {
                    tryNewPath = true;
                    break;
                }
            }
        } while (tryNewPath);
        SpawnCar(paths[randomIndex]);
    }
}
