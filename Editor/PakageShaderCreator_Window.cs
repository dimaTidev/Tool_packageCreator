using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
namespace PackageCreator
{
    public class PakageShaderCreator_Window : EditorWindow
    {
      // #region Window Open
      // //------------------------------------------------------------------------------------------------------------------------
      // [MenuItem("Tools/Package Shader Creator")]
      // static void Init() => ((PakageShaderCreator_Window)EditorWindow.GetWindow(typeof(PakageShaderCreator_Window))).Show();
      // //------------------------------------------------------------------------------------------------------------------------
      // #endregion
      //
      // public UnityEngine.Object saveFolder;      
      //
      // void OnGUI()
      // {
      //     saveFolder = EditorGUILayout.ObjectField("Destination folder", saveFolder, typeof(UnityEngine.Object), true);
      //
      //     GUI.enabled = saveFolder;
      //     if (GUILayout.Button("Create hierarchy"))
      //         CreateHierarchy(AssetDatabase.GetAssetPath(saveFolder));
      //     GUI.enabled = true;
      // }
      //
      // void CreateHierarchy(string path)
      // {
      //     if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
      //         path += Path.DirectorySeparatorChar;
      //
      //     path += "ShaderDevelop/";
      //
      //     if(!Directory.Exists(path + "Shaders"))
      //         Directory.CreateDirectory(path + "Shaders");
      //     if (!Directory.Exists(path + "Cgincs"))
      //         Directory.CreateDirectory(path + "Cgincs");
      //
      //     AssetDatabase.Refresh();
      // }
    }
}
