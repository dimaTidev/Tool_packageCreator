using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUnityVersionAttribute : PropertyAttribute
{
    public bool isUnityVersionOrRevision;
    public EditorUnityVersionAttribute(bool isUnityVersionOrRevision)
    {
        this.isUnityVersionOrRevision = isUnityVersionOrRevision;
    }
}
