using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool {

        public GameObject prefab;
        public GameObject holder;
        public float spacing = 3;
        public float offset = 1;

        const float minSpacing = .1f;

        void Generate () {
            if (pathCreator != null && prefab != null && holder != null) {
                DestroyObjects ();

                VertexPath path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = 0;

                while (dst < path.length) {
                    Vector3 point = path.GetPointAtDistance (dst);
                    Quaternion rot = path.GetRotationAtDistance (dst);
                    Instantiate (prefab, point, rot, holder.transform);
                    dst += spacing;
                }
            }
        }

        void GenerateWithOffset(float offset)
        {
            if (pathCreator != null && prefab != null && holder != null) {
                DestroyObjects ();

                VertexPath path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = 0;

                while (dst < path.length) {
                    Vector3 point = path.GetPointAtDistance (dst);
                    Vector3 toNextPoint;
                    if (dst + spacing >= path.length)
                    {
                        toNextPoint = point - path.GetPointAtDistance(dst - spacing);
                    }
                    else
                    {
                        toNextPoint = path.GetPointAtDistance(dst + spacing) - point;
                    }
                    Debug.Log(dst + spacing + " " + path.length);
                    Debug.Log("To next point: " + toNextPoint);
                    Vector3 move = Quaternion.AngleAxis(90, Vector3.up) * toNextPoint.normalized * offset;
                    Debug.Log("Moving point " + point + " by " + move + " to " + (point + move));
                    Quaternion rot = path.GetRotationAtDistance (dst);
                    Instantiate (prefab, point + move, rot, holder.transform);
                    dst += spacing;
                }
            }
        }

        public List<Vector3> GetPathPoints(float offset=0)
        {            
            List<Vector3> points = new List<Vector3>();

            VertexPath path = pathCreator.path;

            spacing = Mathf.Max(minSpacing, spacing);
            float dst = 0;

            while (dst < path.length) {
                Vector3 point = path.GetPointAtDistance (dst);
                Vector3 toNextPoint;
                if (dst + spacing >= path.length)
                {
                    toNextPoint = point - path.GetPointAtDistance(dst - spacing);
                }
                else
                {
                    toNextPoint = path.GetPointAtDistance(dst + spacing) - point;
                }
                Vector3 move = Quaternion.AngleAxis(90, Vector3.up) * toNextPoint.normalized * offset;
                Quaternion rot = path.GetRotationAtDistance (dst);
                points.Add(point + move);
                dst += spacing;
            }

            return points;
        }

        void DestroyObjects () {
            int numChildren = holder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--) {
                DestroyImmediate (holder.transform.GetChild (i).gameObject, false);
            }
        }

        protected override void PathUpdated () {
            if (pathCreator != null) {
                //Generate ();
                GenerateWithOffset(offset);
            }
        }
    }
}