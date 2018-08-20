using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VDV.Spline;
using VDV.Utility;

namespace VDV.FoggyBoundary
{
    public class BoundaryFogViewer : MonoBehaviour
    {
        [TagSelector] public string BoundaryTag = "foggyBoundary";
        public float FogStartDistance;
        [Range(0, 1)]
        public float MaxFog;
        public AnimationCurve FogCurve;

        private Boundary[] boundary;
        private Transform myTransform;
        private bool inFogRange = false;
        private bool prevFog;
        private FogMode prevFogMode;
        private float prevFogDensity;

        void Start()
        {
            myTransform = transform;
            // FindGameObjectsWithTag doesn't support "Untagged"
            if (BoundaryTag == "Untagged")
            {
                boundary = FindObjectsOfType<Boundary>().Where(go => go.CompareTag(BoundaryTag)).ToArray();
            }
            else
            {
                GameObject[] boundaryGameObjects = GameObject.FindGameObjectsWithTag(BoundaryTag);
                boundary =
                    boundaryGameObjects.Select(go => go.GetComponent<Boundary>())
                        .Where(line => line != null)
                        .ToArray();
            }
        }

        void Update()
        {
            if(boundary == null) return;

            var min = new Helper.LinePoint {Distance = Mathf.Infinity};
            Boundary closestLine = null;
            foreach (Boundary line in boundary)
            {
                Helper.LinePoint point = line.ClosestPoint(myTransform.position);
                if (point.Distance < min.Distance)
                {
                    min = point;
                    closestLine = line;
                }
            }
            if (closestLine != null)
            {
                BoundaryVertex pointA = closestLine.GetPoint(min.Index);
                Vector3 normal = pointA.Normal.normalized;
                if (min.Index < closestLine.PointCount - 1 && min.ProjectionDistance > 0)
                {
                    BoundaryVertex pointB = closestLine.GetPoint(min.Index + 1);
                    normal = Vector3.Lerp(normal, pointB.Normal.normalized, min.ProjectionDistance);
                }
                float dot = Vector3.Dot(myTransform.position - min.Point, normal);
                if (dot > 0)
                {
                    ShowFog(1);
                }
                else if (min.Distance <= FogStartDistance)
                {
                    float fogFactor = 1 - min.Distance / FogStartDistance;
                    dot = Vector3.Dot(myTransform.forward, normal);
                    if (dot <= 0)
                    {
                        ResetFog();
                    }
                    else
                    {
                        fogFactor *= dot;
                        ShowFog(fogFactor);
                    }
                }
                else
                {
                    ResetFog();
                }
            }
            else
            {
                ResetFog();
            }
        }

        private void ResetFog()
        {
            if (inFogRange)
            {
                RenderSettings.fog = prevFog;
                RenderSettings.fogMode = prevFogMode;
                RenderSettings.fogDensity = prevFogDensity;
            }
            inFogRange = false;
        }

        private void ShowFog(float fogFactor)
        {
            if (!inFogRange)
            {
                prevFog = RenderSettings.fog;
                prevFogMode = RenderSettings.fogMode;
                prevFogDensity = RenderSettings.fogDensity;
            }
            inFogRange = true;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = Mathf.Clamp01(Mathf.Lerp(prevFogDensity, MaxFog, FogCurve.Evaluate(fogFactor)));
        }

        void OnValidate()
        {
            FogStartDistance = Mathf.Max(0, FogStartDistance);
        }
    }
}