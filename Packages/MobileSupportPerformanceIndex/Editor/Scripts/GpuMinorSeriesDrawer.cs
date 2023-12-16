// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MobileSupport.PerformanceIndex.Editor
{
    [CustomPropertyDrawer(typeof(GpuSeriesEnumeration))]
    public class GpuSeriesEnumerationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // get enum values of GpuSeriesEnumeration
            var enums = GpuSeriesEnumeration.GetAll<GpuSeriesEnumeration>().ToArray();
            var enumNames = enums.Select(e => e.ToString()).ToArray();

            // get value and index
            var valueProperty = property.FindPropertyRelative("value");
            var selectedValue = valueProperty.intValue;
            var selectedEnum = (GpuSeriesEnumeration)selectedValue;
            var previousIndex = Array.IndexOf(enums, selectedEnum);

            // draw enum selection popup
            var selectedIndex = EditorGUI.Popup(position, previousIndex, enumNames);

            if (selectedIndex != previousIndex)
            {
                // update SerializedProperty value
                selectedEnum = enums[selectedIndex];
                valueProperty.intValue = (int)selectedEnum;
                property.FindPropertyRelative("gpuMajorSeries").intValue = (int)selectedEnum.GpuMajorSeries;
                property.FindPropertyRelative("gpuMinorSeries").intValue = (int)selectedEnum.GpuMinorSeries;
                property.serializedObject.ApplyModifiedProperties();
            }

            // restore indent
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
