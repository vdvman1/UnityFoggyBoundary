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

        protected override void OnRenderPoint(int index, BoundaryVertex point, Vector3 pos, float handleSize)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 destination = Handles.DoPositionHandle(pos + point.Normal, HandleTransform);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Line, "Move Normal");
                EditorUtility.SetDirty(Line);
                point.Normal = (destination - point.Position).normalized;
                Line.SetPoint(index, point);
            }
            if (Event.current.type == EventType.Repaint)
            {
                Handles.ArrowHandleCap(GUIUtility.GetControlID(FocusType.Passive), pos, Quaternion.LookRotation(point.Normal), 1, EventType.Repaint);
            }
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
    }
}
