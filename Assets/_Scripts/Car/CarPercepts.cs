using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPercepts : MonoBehaviour
{
    public class RaycastInfo {
        public float forwardOffset;
        public float sidewaysOffset;
        public float verticalOffset;
        public float angleFromForward;
        public float distance;
        public Vector3 hitPoint;
        public GameObject hitObject;

        public RaycastInfo(float forwardOffset, float sidewaysOffset, float verticalOffset,
                           float angleFromForward)
        {
            this.forwardOffset = forwardOffset;
            this.sidewaysOffset = sidewaysOffset;
            this.verticalOffset = verticalOffset;
            this.angleFromForward = angleFromForward;
            this.distance = 0f;
            this.hitPoint = Vector3.zero;
            this.hitObject = null;
        }

        public Vector3 GetOrigin(Transform transform) {
            return transform.position + transform.forward * forwardOffset +
                transform.right * sidewaysOffset + transform.up * verticalOffset;
        }

        public Vector3 GetDirection(Transform transform) {
            return Quaternion.Euler(0, angleFromForward, 0) * transform.forward;
        }
    }

    [Header("Car Raycast Configuration")]
    [SerializeField][Range(1, 20)]
    [Tooltip("Number of pairs of rays to cast in addition to one ray in the center")]
    private int numRaycastPairs = 15;
    [SerializeField]
    [Tooltip("The length of the rays cast in units (default 10)")]
    private float _rayLength = 10f;
    [SerializeField][Range(1, 120)]
    [Tooltip("Raycasts will be cast evenly spaced around the car's front up to this angle")]
    private float maxRaycastAngle = 85f;
    [SerializeField]
    [Tooltip("The distance between the center and the front of the car")]
    private float _forwardRaycastOffset = 0.75f;
    [SerializeField]
    [Tooltip("The y distance between the car's origin and where the raycast originates")]
    private float _verticalRaycastOffset = -0.1f;

    [Header("Car Perception Parameters")]
    [Tooltip("The distance from which a car can see a traffic signal at the end of its current path")]
    public float trafficSignalPerceptionDistance = 2f;

    private PathCrawler _pathCrawler;
    private bool _collidedWithObject = false;
    private string _collidedWithObjectTag;

    [HideInInspector]
    public int approachingTrafficSignalType = -1;
    public List<RaycastInfo> raycasts;
    [HideInInspector]
    public List<float> raycastCollisionDistances;

    void Start()
    {
        raycasts = new List<RaycastInfo>();
        HandleRaycastCountChange();
        raycastCollisionDistances = new List<float>();

        foreach (RaycastInfo raycast in raycasts)
        {
            raycastCollisionDistances.Add(-1);
        }

        if (!TryGetComponent<PathCrawler>(out _pathCrawler))
        {
            Debug.Log("CarPercepts must be attached to a PathCrawler");
        }
    }

    void FixedUpdate()
    {
        HandleRaycastCountChange();
        CheckCollisions();
        GetCollisions(out raycastCollisionDistances);
        // DrawDebugLines();

        if (_pathCrawler != null && _pathCrawler.currentPath != null) {
            CheckApproachingTrafficSignal();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag != "Street")
        {
            _collidedWithObject = true;
            _collidedWithObjectTag = other.gameObject.tag;
        }
    }

    public bool CollidedWithObject(out string tag, bool clear = false)
    {
        tag = "";
        if (_collidedWithObject)
        {
            if (clear)
            {
                _collidedWithObject = false;
            }
            tag = _collidedWithObjectTag;
            return true;
        }
        return false;
    }

    public bool GetCollisions(out List<float> distances, string objTag="") {
        distances = new List<float>();
        foreach (RaycastInfo raycast in raycasts) {
            if (raycast.hitObject != null && raycast.hitObject != transform.GetChild(0).gameObject) {
                if (objTag == "" || raycast.hitObject.tag == objTag) {
                    distances.Add(raycast.distance);
                    continue;
                }
            }
            distances.Add(-1);
        }
        return distances.Count > 0;
    }

    void CheckCollisions()
    {
        foreach (RaycastInfo raycast in raycasts)
        {
            RaycastHit hit;
            if (Physics.Raycast(raycast.GetOrigin(transform), raycast.GetDirection(transform), out hit,
                                _rayLength))
            {
                if (hit.collider.gameObject != transform.GetChild(0).gameObject)
                {
                    raycast.hitObject = hit.collider.gameObject;
                    raycast.distance = hit.distance;
                    raycast.hitPoint = hit.point;
                    continue;
                }
            }
            raycast.hitObject = null;
            raycast.distance = -1;
            raycast.hitPoint = Vector3.zero;
        }
    }

    void CheckApproachingTrafficSignal()
    {
        if (_pathCrawler.currentPath.connectedTrafficSignal != null) {
            TrafficSignalGroup trafficSignal = _pathCrawler.currentPath.connectedTrafficSignal;
            if (trafficSignal != null)
            {
                approachingTrafficSignalType = trafficSignal.signalType;
            }
        }
    }

    public bool CheckStopForTrafficSignal(out float distance)
    {
        if (approachingTrafficSignalType == 0 || approachingTrafficSignalType == 3)
        {
            Vector3[] pathNodes = _pathCrawler.currentPath.nodes;
            Vector3 lastNodePos = pathNodes[pathNodes.Length - 1];
            float distanceToLastNode = Vector3.Distance(transform.position, lastNodePos);
            if (distanceToLastNode < trafficSignalPerceptionDistance)
            {
                distance = distanceToLastNode;
                return true;
            }
        }
        distance = -1;
        return false;
    }

    public Vector3 GetTrafficSignalNodePosition()
    {
        if (approachingTrafficSignalType != -1)
        {
            Vector3[] pathNodes = _pathCrawler.currentPath.nodes;
            Vector3 lastNodePos = pathNodes[pathNodes.Length - 1];
            return lastNodePos;
        }
        return Vector3.zero;
    }

    void HandleRaycastCountChange()
    {
        if (raycasts.Count != numRaycastPairs * 2 + 1 ||
            raycasts[raycasts.Count - 1].angleFromForward != maxRaycastAngle)
        {
            raycasts.Clear();
            raycasts.Add(new RaycastInfo(_forwardRaycastOffset, 0, _verticalRaycastOffset, 0));
            for (int i = 0; i < numRaycastPairs; i++)
            {
                float angle = (i + 1) / (float)numRaycastPairs * maxRaycastAngle;
                raycasts.Add(new RaycastInfo(_forwardRaycastOffset, 0, _verticalRaycastOffset, angle));
                raycasts.Add(new RaycastInfo(_forwardRaycastOffset, 0, _verticalRaycastOffset, -angle));
            }
        }
    }

    void DrawDebugLines()
    {
        foreach (RaycastInfo raycast in raycasts)
        {
            Color color = raycast.hitObject == null ? Color.red : Color.green;
            Vector3 origin = raycast.GetOrigin(transform);
            Vector3 endPoint = origin + raycast.GetDirection(transform).normalized * _rayLength;
            Debug.DrawLine(origin, endPoint, color);
        }
    }
}
