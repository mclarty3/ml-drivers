using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCrawler : MonoBehaviour
{
    public Path currentPath;
    public int currentNodeIndex;
    public float speed = 0.5f;
    public float nodeTriggerDistance = 0.1f;
    private Vector3 currentNodePosition;
    public float timeBetweenNodes = 2f;
    private float time;
    private 

    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        transform.position = currentPath.nodes[currentNodeIndex];
        MoveToNextNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPath == null) {
            return;
        }

        if (Vector3.Distance(transform.position, currentNodePosition) <=  nodeTriggerDistance) {
            MoveToNextNode();
        }

        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public void MoveToNextNode()
    {
        currentNodeIndex++;
        if (currentNodeIndex >= currentPath.NumNodes)
        {
            currentNodeIndex = 0;
            currentPath = currentPath.GetConnectingPath();
        }
        currentNodePosition = currentPath.nodes[currentNodeIndex];
        transform.rotation = Quaternion.LookRotation(currentNodePosition - transform.position);
    }
}
