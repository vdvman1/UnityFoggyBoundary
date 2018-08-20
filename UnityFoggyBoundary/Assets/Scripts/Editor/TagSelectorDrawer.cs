using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VDV.Utility
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var selector = attribute as TagSelectorAttribute;
            if (selector == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

            int index = -1;
            string currentTag = property.stringValue;
            index = Array.IndexOf(tags, currentTag);
            index = EditorGUI.Popup(position, label.text, index, tags);
            if (index >= 0 && index < tags.Length)
            {
                property.stringValue = tags[index];
            }

            EditorGUI.EndProperty();
        }
    }
}
