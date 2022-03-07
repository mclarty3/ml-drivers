using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPercepts : MonoBehaviour
{
    public float forwardRaycastOffset = 0.55f;
    public float sideRaycastOffset = 0.225f;
    public float verticalRaycastOffset = -0.1f;
    public float rayLength = 10f;

    public class Raycast {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;
        public Vector3 hitPoint;
        public GameObject hitObject;

        public Raycast(Vector3 origin, Vector3 direction, float distance) {
            this.origin = origin;
            this.direction = direction;
            this.distance = distance;
            this.hitPoint = Vector3.zero;
            this.hitObject = null;
        }
    }

    public List<Raycast> raycasts;

    // Start is called before the first frame update
    void Start()
    {
        raycasts = new List<Raycast>()
        {
            new Raycast(transform.position + new Vector3(0, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset/2, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right * 0.25f, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset/2, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right * 0.5f, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right * 2, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right * 4, rayLength),
            new Raycast(transform.position + new Vector3(sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward + transform.right * 8, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset/2, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right * 0.25f, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset/2, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right * 0.5f, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right * 2, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right * 4, rayLength),
            new Raycast(transform.position + new Vector3(-sideRaycastOffset, verticalRaycastOffset, forwardRaycastOffset),
                        transform.forward - transform.right * 8, rayLength),
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        CheckCollisions();
        DrawDebugLines();
    }

    public bool GetCollisions(out List<GameObject> collisions, out List<float> distances, string objTag="") {
        collisions = new List<GameObject>();
        distances = new List<float>();
        foreach (Raycast raycast in raycasts) {
            if (raycast.hitObject != null) {
                if (objTag != "" && raycast.hitObject.tag != objTag) {
                    continue;
                }
                collisions.Add(raycast.hitObject);
                distances.Add(raycast.distance);
            }
        }

        return collisions.Count > 0;
    }

    void CheckCollisions()
    {
        foreach (Raycast raycast in raycasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(raycast.origin, raycast.direction, out hit, raycast.distance))
            {
                raycast.distance = hit.distance;
                raycast.hitPoint= hit.point;
                raycast.hitObject = hit.collider.gameObject;
            }
            else if (raycast.hitObject != null)
            {
                raycast.hitObject = null;
            }
        }
    }

    void DrawDebugLines()
    {
        foreach (Raycast raycast in raycasts)
        {
            Color color = raycast.hitObject == null ? Color.red : Color.green;
            Vector3 endPoint = raycast.origin + raycast.direction.normalized * raycast.distance;
            Debug.DrawLine(raycast.origin, endPoint, color);
        }
    }
}
