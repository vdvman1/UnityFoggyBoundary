using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
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
        public float SlowdownEndDistance = 10;
        public AnimationCurve SlowdownCurve;

        private Boundary[] boundary;
        private Transform myTransform;
        private bool inFogRange = false;
        private bool prevFog;
        private FogMode prevFogMode;
        private float prevFogDensity;

        private FirstPersonController fpsController;
        private FieldInfo walkSpeedField;
        private FieldInfo runSpeedField;
        private FieldInfo moveDirField;

        private float prevWalkSpeed = 0;
        private float prevRunSpeed = 0;

        void Start()
        {
            myTransform = transform;
            fpsController = GetComponent<FirstPersonController>();
            Type type = typeof(FirstPersonController);
            walkSpeedField = type.GetField("m_WalkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            runSpeedField = type.GetField("m_RunSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            moveDirField = type.GetField("m_MoveDir", BindingFlags.NonPublic | BindingFlags.Instance);

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
                normal = closestLine.transform.TransformDirection(normal);
                float lookDot = Vector3.Dot(myTransform.forward, normal);
                float dot = Vector3.Dot(myTransform.position - min.Point, normal);
                if (lookDot <= 0)
                {
                    ResetFog();
                }
                else if (dot > 0)
                {
                    ShowFog(lookDot);
                    if (fpsController != null)
                    {
                        Slowdown(min.Distance, normal, lookDot);
                    }
                }
                else if (min.Distance <= FogStartDistance)
                {
                    float fogFactor = 1 - min.Distance / FogStartDistance;
                    fogFactor *= lookDot;
                    ShowFog(fogFactor);
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
                if (fpsController != null)
                {
                    walkSpeedField.SetValue(fpsController, prevWalkSpeed);
                    runSpeedField.SetValue(fpsController, prevRunSpeed);
                }
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
                if (fpsController != null)
                {
                    prevWalkSpeed = (float) walkSpeedField.GetValue(fpsController);
                    prevRunSpeed = (float) runSpeedField.GetValue(fpsController);
                }
            }
            inFogRange = true;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = Mathf.Clamp01(Mathf.Lerp(prevFogDensity, MaxFog, FogCurve.Evaluate(fogFactor)));
        }

        private void Slowdown(float distance, Vector3 normal, float lookDot)
        {
            float slowdownFactor = distance / SlowdownEndDistance;
            slowdownFactor = Mathf.Clamp01(slowdownFactor);
            var moveDir = (Vector3) moveDirField.GetValue(fpsController);
            moveDir.Normalize();
            Debug.Log(moveDir);
            float dot = Vector3.Dot(normal, moveDir);
            slowdownFactor *= lookDot;
            Debug.Log(lookDot);
            slowdownFactor = SlowdownCurve.Evaluate(slowdownFactor);
            walkSpeedField.SetValue(fpsController, Mathf.Lerp(prevWalkSpeed, 0, slowdownFactor));
            runSpeedField.SetValue(fpsController, Mathf.Lerp(prevRunSpeed, 0, slowdownFactor));
        }

        void OnValidate()
        {
            FogStartDistance = Mathf.Max(0, FogStartDistance);
        }
    }
}