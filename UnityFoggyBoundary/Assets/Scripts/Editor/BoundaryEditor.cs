using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VDV.Spline.Editor;

namespace VDV.FoggyBoundary.Editor
{
    [CustomEditor(typeof(Boundary))]
    public class BoundaryEditor : GenericLineEditor<Boundary, BoundaryVertex>
    {
        private int selectedNormalIndex = -1;

        protected override void OnSelectPoint()
        {
            selectedNormalIndex = -1;
        }

        protected override bool ShouldRenderPositionHandleForPoint(int index)
        {
            return base.ShouldRenderPositionHandleForPoint(index) && selectedNormalIndex == -1;
        }

        protected override void OnRenderPoint(int index, BoundaryVertex point, Vector3 pos, float handleSize)
        {
            EventType eventType = Event.current.type;
            int arrowId = GUIUtility.GetControlID(FocusType.Passive);
            Vector3 dir = LineTransform.TransformDirection(point.Normal);
            RenderNormal(pos, dir, eventType, arrowId);
            if (eventType == EventType.MouseDown)
            {
                if (HandleUtility.nearestControl == arrowId)
                {
                    selectedNormalIndex = index;
                    SelectPoint(index, notify: false);
                }
            }
            if (selectedNormalIndex != index) return;

            EditorGUI.BeginChangeCheck();
            Vector3 destination = Handles.DoPositionHandle(pos + dir, HandleTransform);
            destination = LineTransform.InverseTransformPoint(destination);
            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject(Line, "Move Normal");
            EditorUtility.SetDirty(Line);
            point.Normal = destination - point.Position;
            float sqrMagnitude = point.Normal.sqrMagnitude;
            // Don't normalize if less that 1 to avoid glitchy movement
            if (sqrMagnitude > 1)
            {
                point.Normal /= Mathf.Sqrt(sqrMagnitude);
            }
            Line.SetPoint(index, point);
        }

        private void RenderNormal(Vector3 pos, Vector3 normal, EventType eventType, int arrowId)
        {
            Handles.ArrowHandleCap(arrowId, pos, Quaternion.LookRotation(normal), 1, eventType);
        }

        protected override void OnInspectorPoint(int index, BoundaryVertex point)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 normal = EditorGUILayout.Vector3Field("Normal", point.Normal);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Line, "Move Normal");
                EditorUtility.SetDirty(Line);
                point.Normal = normal.normalized;
                Line.SetPoint(SelectedPointIdx, point);
            }
        }

        protected override bool AfterLoopToggleBox(bool newLoop)
        {
            EditorGUILayout.HelpBox("Boundaries that are not closed can cause jumps in the amount of fog when a player leaves the boundary.", MessageType.Warning);
            return true;
        }
        
        protected override string Title()
        {
            return "Boundary";
        }
    }
}
