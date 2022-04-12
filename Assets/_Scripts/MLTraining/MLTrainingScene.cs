using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTrainingScene : MonoBehaviour
{

    public GameObject roadsParent;
    public GameObject driverAgentPrefab;

    private NodePath[] _nodePaths;
    private MLDriverAgent _driverAgent;

    // Start is called before the first frame update
    void Start()
    {
        _nodePaths = roadsParent.transform.GetComponentsInChildren<NodePath>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeScene(MLDriverAgent agent)
    {
        NodePath startingPath = _nodePaths[Random.Range(0, _nodePaths.Length)];
        Quaternion startingRot = Quaternion.LookRotation(startingPath.nodes[1] - startingPath.nodes[0]);

        agent.transform.position = startingPath.nodes[0];
        agent.transform.position += agent.transform.forward * -0.5f;
        agent.transform.rotation = startingRot;
        agent.transform.parent = transform;
        agent.GetComponent<PathCrawler>().Initialize(startingPath);
        agent.GetComponent<Rigidbody>().velocity = Vector3.zero;

        _driverAgent = agent;
    }
}
