using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VDV.Spline;
using VDV.Utility;

namespace VDV.FoggyBoundary
{
    public class BoundaryFogViewer: MonoBehaviour
    {
        [TagSelector]
        public string BoundaryTag = "foggyBoundary";
        public float FogStartDistance;
        public float FogEndDistanceMinimum;
        public float FogEndDistanceMaximum;
        public float FogStartDistanceMinimum;
        public float FogStartDistanceMaximum;

        private Boundary[] boundary;
        private Transform myTransform;
        private bool inFogRange = false;
        private bool prevFog;
        private FogMode prevFogMode;
        private float prevFogStartDistance;
        private float prevFogEndDistance;

        void Start()
        {
            myTransform = transform;
        }

        void Update()
        {
            if (boundary == null)
            {
                // FindGameObjectsWithTag doesn't support "Untagged"
                if (BoundaryTag == "Untagged")
                {
                    boundary = FindObjectsOfType<Boundary>().Where(go => go.tag == BoundaryTag).ToArray();
                }
                else
                {
                    GameObject[] boundaryGameObjects = GameObject.FindGameObjectsWithTag(BoundaryTag);
                    boundary = boundaryGameObjects.Select(go => go.GetComponent<Boundary>()).Where(line => line != null).ToArray();
                }
            }

            var min = new Helper.LinePoint {Distance = Mathf.Infinity};
            foreach (Boundary line in boundary)
            {
                Helper.LinePoint point = line.ClosestPoint(myTransform.position);
                if (point.Distance < min.Distance)
                {
                    min = point;
                }
            }
            if (min.Distance <= FogStartDistance)
            {
//                Debug.Log(min.Distance);
                float fogFactor = min.Distance / FogStartDistance;
                if (!inFogRange)
                {
                    prevFog = RenderSettings.fog;
                    prevFogMode = RenderSettings.fogMode;
                    prevFogStartDistance = RenderSettings.fogStartDistance;
                    prevFogEndDistance = RenderSettings.fogEndDistance;
                }
                inFogRange = true;
                RenderSettings.fog = true;
                RenderSettings.fogStartDistance = Mathf.Lerp(FogStartDistanceMinimum, FogStartDistanceMaximum, fogFactor);
                RenderSettings.fogEndDistance = Mathf.Lerp(FogEndDistanceMinimum, FogEndDistanceMaximum, fogFactor);
                Debug.Log(string.Format("fogStartDistance: {0}, fogEndDistance: {1}", RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
                Debug.Log(fogFactor);
            }
            else
            {
                if (inFogRange)
                {
                    RenderSettings.fog = prevFog;
                    RenderSettings.fogMode = prevFogMode;
                    RenderSettings.fogStartDistance = prevFogStartDistance;
                    RenderSettings.fogEndDistance = prevFogEndDistance;
                }
                inFogRange = false;
            }
        }

#if UNITY_EDITOR
        public void OnPropertyChanged(UnityEditor.SerializedProperty prop)
        {
            switch (prop.name)
            {
                case "BoundaryTag":
                    BoundaryTag = prop.stringValue;
                    break;
                case "FogStartDistance":
                    FogStartDistance = Mathf.Max(prop.floatValue, 0);
                    break;
                case "FogEndDistanceMinimum":
                    FogEndDistanceMinimum = Mathf.Max(prop.floatValue, 0);
                    FogEndDistanceMaximum = Mathf.Max(FogEndDistanceMinimum, FogEndDistanceMaximum);
                    FogStartDistanceMinimum = Mathf.Min(FogStartDistanceMinimum, FogEndDistanceMinimum);
                    break;
                case "FogEndDistanceMaximum":
                    FogEndDistanceMaximum = Mathf.Max(prop.floatValue, 0);
                    FogEndDistanceMinimum = Mathf.Min(FogEndDistanceMinimum, FogEndDistanceMaximum);
                    FogStartDistanceMaximum = Mathf.Min(FogStartDistanceMaximum, FogEndDistanceMaximum);
                    FogStartDistanceMinimum = Mathf.Min(FogStartDistanceMinimum, FogEndDistanceMaximum);
                    break;
                case "FogStartDistanceMinimum":
                    FogStartDistanceMinimum = Mathf.Max(prop.floatValue, 0);
                    FogStartDistanceMaximum = Mathf.Max(FogStartDistanceMaximum, FogStartDistanceMinimum);
                    FogEndDistanceMaximum = Mathf.Max(FogEndDistanceMaximum, FogStartDistanceMinimum);
                    FogEndDistanceMinimum = Mathf.Max(FogEndDistanceMinimum, FogStartDistanceMinimum);
                    break;
                case "FogStartDistanceMaximum":
                    FogStartDistanceMaximum = Mathf.Max(prop.floatValue, 0);
                    FogStartDistanceMinimum = Mathf.Min(FogStartDistanceMinimum, FogStartDistanceMaximum);
                    FogEndDistanceMaximum = Mathf.Max(FogEndDistanceMaximum, FogStartDistanceMaximum);
                    break;
            }
        }
    }
#endif
}
