using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VDV.FoggyBoundary
{
    [CustomEditor(typeof(BoundaryFogViewer))]
    public class FoggyBoundaryViewerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var viewer = target as BoundaryFogViewer;
            if(viewer == null) return;

            for (SerializedProperty prop = serializedObject.GetIterator(); prop.NextVisible(true);)
            {
                string propName = prop.name;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
                if (EditorGUI.EndChangeCheck())
                {
                    viewer.OnPropertyChanged(prop);
                }
                switch (propName)
                {
                    case "FogEndDistanceMaximum":
                        if (RenderSettings.fogMode == FogMode.Linear && !Mathf.Approximately(RenderSettings.fogEndDistance, prop.floatValue))
                        {
                            EditorGUILayout.HelpBox("This value may cause a sudden jump in fog when approaching the boundary.", MessageType.Warning);
                        }
                        break;
                    case "FogStartDistanceMaximum":
                        if (RenderSettings.fogMode == FogMode.Linear && !Mathf.Approximately(RenderSettings.fogStartDistance, prop.floatValue))
                        {
                            EditorGUILayout.HelpBox("This value may cause a sudden jump in fog when approaching the boundary.", MessageType.Warning);
                        }
                        break;
                }
            }
            if (RenderSettings.fog && RenderSettings.fogMode != FogMode.Linear)
            {
                EditorGUILayout.HelpBox("The standard fog is not linear, this may cause a sudden jump in fog when approaching the world boundary", MessageType.Warning);
            }
        }
    } 
}
