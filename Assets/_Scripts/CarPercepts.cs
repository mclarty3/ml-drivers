using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPercepts : MonoBehaviour
{
    public float forwardRaycastOffset = 0.55f;
    public float sideRaycastOffset = 0.225f;
    public float verticalRaycastOffset = -0.1f;
    public float rayLength = 10f;

    public class RaycastInfo {
        public float forwardOffset;
        public float sidewaysOffset;
        public float angleFromForward;
        public float distance;
        public Vector3 hitPoint;
        public GameObject hitObject;

        public RaycastInfo(float forwardOffset, float sidewaysOffset,
                           float angleFromForward)
        {
            this.forwardOffset = forwardOffset;
            this.sidewaysOffset = sidewaysOffset;
            this.angleFromForward = angleFromForward;
            this.distance = 0f;
            this.hitPoint = Vector3.zero;
            this.hitObject = null;
        }

        public Vector3 GetOrigin(Transform transform) {
            return transform.position + transform.forward * forwardOffset +
                transform.right * sidewaysOffset;
        }

        public Vector3 GetDirection(Transform transform) {
            return Quaternion.Euler(0, angleFromForward, 0) * transform.forward;
        }
    }

    public List<RaycastInfo> raycasts;

    void Start()
    {
        raycasts = new List<RaycastInfo>()
        {
            new RaycastInfo(forwardRaycastOffset, 0, 0),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset/2, 15),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset/2, 30),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset/2, 45),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset, 60),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset, 75),
            new RaycastInfo(forwardRaycastOffset, sideRaycastOffset, 85),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset/2, -15),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset/2, -30),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset/2, -45),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset, -60),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset, -75),
            new RaycastInfo(forwardRaycastOffset, -sideRaycastOffset, -85),
        };
    }

    void FixedUpdate()
    {
        CheckCollisions();
        DrawDebugLines();
    }

    public bool GetCollisions(out List<GameObject> collisions, out List<float> distances, string objTag="") {
        collisions = new List<GameObject>();
        distances = new List<float>();
        foreach (RaycastInfo raycast in raycasts) {
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
        foreach (RaycastInfo raycast in raycasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(raycast.GetOrigin(transform), raycast.GetDirection(transform), out hit,
                                rayLength))
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
        foreach (RaycastInfo raycast in raycasts)
        {
            Color color = raycast.hitObject == null ? Color.red : Color.green;
            Vector3 origin = raycast.GetOrigin(transform);
            Vector3 endPoint = origin + raycast.GetDirection(transform).normalized * rayLength;
            Debug.DrawLine(origin, endPoint, color);
        }
    }
}
