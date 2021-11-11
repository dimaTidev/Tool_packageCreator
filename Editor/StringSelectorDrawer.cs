using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(StringSelectorAttribute))]
public class StringSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        StringSelectorAttribute attr = attribute as StringSelectorAttribute;

        Rect rect = new Rect(position.x, position.y, position.width / 2, position.height);

        if (attr.isShowProperty)
        {
            EditorGUI.PropertyField(rect, property); //label
            rect.x += rect.width;
        } 

        int index = Array.IndexOf(attr.array, property.stringValue);

        if (attr.isEnumOrButton)
        {
            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(rect, index, attr.array);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = attr.array[index];
        }
        else
        {
            int buttonWidth = (int)(rect.width / (float)attr.array.Length);

            rect.x -= buttonWidth;
            rect.width = buttonWidth;

            for (int i = 0; i < attr.array.Length; i++)
            {
                if (i == index)
                    GUI.color = Color.yellow;
                rect.x += buttonWidth;
                if (GUI.Button(rect, attr.array[i]))
                    property.stringValue = attr.array[i];
                
                GUI.color = Color.white;
            }
        }
        
    }
}
