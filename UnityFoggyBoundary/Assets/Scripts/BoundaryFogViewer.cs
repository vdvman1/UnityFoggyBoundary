using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VDV.Spline;

namespace VDV.FoggyBoundary
{
    public class BoundaryFogViewer: MonoBehaviour
    {
        public string BoundaryTag = "foggyBoundary";
        public float fogStartDistance;
        [Range(0,1)]
        public float minFogDensity;
        [Range(0,1)]
        public float maxFogDensity;

        private Line[] boundary;
        private Transform myTransform;

        void Start()
        {
            myTransform = transform;
        }

        void Update()
        {
            if (boundary == null)
            {
                GameObject[] boundaryGameObjects = GameObject.FindGameObjectsWithTag(BoundaryTag);
                boundary = boundaryGameObjects.Select(go => go.GetComponent<Line>()).Where(line => line != null).ToArray();
            }
            Line closestLine = null;
            float minSqrDistance = Mathf.Infinity;
            foreach (Line line in boundary)
            {
                float lineMinSqrDistance = line.MinSqrDistanceFrom(myTransform.position);
                if (lineMinSqrDistance < minSqrDistance)
                {
                    minSqrDistance = lineMinSqrDistance;
                    closestLine = line;
                }
            }
            float minDistance = Mathf.Sqrt(minSqrDistance);
            Debug.Log(minDistance);
            float fog = 0;
            if (minDistance <= fogStartDistance)
            {
                fog = 1 - minDistance / fogStartDistance;
                fog *= maxFogDensity - minFogDensity;
            }
            fog += minFogDensity;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = fog;
        }
    } 
}
