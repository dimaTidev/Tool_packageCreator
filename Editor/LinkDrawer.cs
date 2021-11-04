using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LinkAttribute))]
public class LinkDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int buttonWidth = 25;
        Rect rect = new Rect(position.x, position.y, position.width - buttonWidth, position.height);
        EditorGUI.PropertyField(rect, property); //label

        rect.x += position.width - buttonWidth;
        rect.width = buttonWidth;
        if (GUI.Button(rect, EditorGUIUtility.IconContent("CollabPush")))
            Application.OpenURL(property.stringValue);
    }
   
}
