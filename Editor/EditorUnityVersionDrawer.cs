using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(EditorUnityVersionAttribute))]
public class EditorUnityVersionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorUnityVersionAttribute attr = attribute as EditorUnityVersionAttribute;

        float widthButton = 40;
        float width = position.width / 3;

        Rect rect = new Rect(position.x, position.y, 250, position.height);

        EditorGUI.PropertyField(rect, property);

        rect.x += rect.width;
        rect.width = widthButton;

        if (GUI.Button(rect, "Set"))
        {
            property.stringValue = GetVers(attr.isUnityVersionOrRevision);
        }
            

        rect.x += rect.width;
        rect.width = width;

        GUI.Label(rect, GetVers(attr.isUnityVersionOrRevision));
    }

    string GetVers(bool isVersionOrRevision)
    {
        string uVersion = Application.unityVersion;
        string uVer = uVersion.Remove(uVersion.LastIndexOf("."));
        string uRev = uVersion.Remove(0, uVersion.LastIndexOf(".") + 1);
        return isVersionOrRevision ? uVer : uRev;
    }
}
