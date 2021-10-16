using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoPackageCreator_Window : EditorWindow
{
    static SerializedObject serializedObject;

    string 
        _name = "com.accName.repoName",
        displName = "Test repo",
        version = "0.0.1",
        unityVers = "2018.4",
        descr = "",
        author = "DimaTi <timofeenkodima@gmail.com> (https://github.com/dimaTidev)",
        //keywords,
        category;

    public UnityEngine.Object saveFolder;

    bool
        folder_Editor,
        folder_Runtime = true,
        folder_TestEditor,
        folder_TestRuntime,
        folder_Samples,
        folder_Documentation;

    bool foldoutExamples;


    [System.Serializable]
    public class RepositoryData
    {
        public string 
            type = "git", 
            url = "https://github.com/dimaTidev/Package_MethodsReflection.git";
    }
    public RepositoryData repoData;
    public List<string> dependencies = new List<string>();
   // [AssetMenu]
    [MenuItem("Tools/Package Creator")]
    static void Init()
    {
        AutoPackageCreator_Window window = (AutoPackageCreator_Window)EditorWindow.GetWindow(typeof(AutoPackageCreator_Window));
        window.Show();

        serializedObject = new SerializedObject(window);
    }
    void OnGUI()
    {
        Draw_SaveFolder();

        GUI.enabled = saveFolder;

        EditorGUILayout.BeginHorizontal(); //--------

        EditorGUILayout.BeginVertical("Helpbox"); //~~~

        _name = EditorGUILayout.TextField("name", _name);

        if (saveFolder)
        {
            string path = AssetDatabase.GetAssetPath(saveFolder) + "/" + _name;
            if (Directory.Exists(path))
            {
                Debug.Log("Contain folder");
                path += "/" + "package.json";
                if (File.Exists(path))
                {
                    Debug.Log("Contain file");

                    EditorGUILayout.LabelField("folder already contain package.json!");
                    GUI.enabled = false;
                }
            }
        }
        DrawLines();
        Draw_Repo();
        Draw_Dependencies();
        EditorGUILayout.EndVertical(); //~~~

        GUI.enabled = saveFolder;

        EditorGUILayout.BeginVertical("Helpbox"); //~~~
        Draw_FoldersToggles();
        EditorGUILayout.EndVertical(); //~~~

        EditorGUILayout.EndHorizontal();//----------

        GUI.enabled = true;

        EditorGUILayout.Space();
        foldoutExamples = EditorGUILayout.Foldout(foldoutExamples, "Examples");
        if(foldoutExamples)
            Draw_Examples();

        GUI.enabled = saveFolder;
        GUI.color = Color.yellow;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Create hierarhy"))
        {
            CreateFiles(AssetDatabase.GetAssetPath(saveFolder));
        }
        GUI.color = Color.white;
        GUI.enabled = true;
    }

    #region Drawers
    void DrawLines()
    {
        displName = EditorGUILayout.TextField("displayName", displName);
        version = EditorGUILayout.TextField("version", version);
        unityVers = EditorGUILayout.TextField("unity", unityVers);
        author = EditorGUILayout.TextField("author", author);
        category = EditorGUILayout.TextField("category", category);
        // EditorGUILayout.TextArea("description", descr);
        EditorGUILayout.LabelField("description");
        descr = EditorGUILayout.TextArea(descr);
    }
    void Draw_FoldersToggles()
    {
        folder_Editor = EditorGUILayout.Toggle("folder Editor", folder_Editor);
        folder_Runtime = EditorGUILayout.Toggle("folder Runtime", folder_Runtime);
       // isFolderTest = EditorGUILayout.Toggle("isFolderTest", isFolderTest);

       // bool guiTemp = GUI.enabled;
       // GUI.enabled = isFolderTest;
        folder_TestEditor = EditorGUILayout.Toggle("folder TestEditor", folder_TestEditor);
        folder_TestRuntime = EditorGUILayout.Toggle("folder TestRuntime", folder_TestRuntime);
       // GUI.enabled = guiTemp;

        folder_Samples = EditorGUILayout.Toggle("folder Samples", folder_Samples);
        folder_Documentation = EditorGUILayout.Toggle("folder Documentation", folder_Documentation);
    }
    void Draw_SaveFolder()
    {
        saveFolder = EditorGUILayout.ObjectField("Destination folder", saveFolder, typeof(UnityEngine.Object), true);
    }
    void Draw_Repo()
    {
        EditorGUILayout.Space();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);

        SerializedProperty list = so.FindProperty("repoData");
        EditorGUILayout.PropertyField(list, true);
        so.ApplyModifiedProperties();
    }
    void Draw_Dependencies()
    {
        EditorGUILayout.Space();

        

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);

        SerializedProperty list = so.FindProperty("dependencies");
        EditorGUILayout.PropertyField(list, true);
        so.ApplyModifiedProperties();
    }
    void Draw_Examples()
    {
        GUILayout.Label("----Examples----------------------------");
        string depEx1 = "com.mycompany.mypackage2\": \"ssh://git@github.example.com/myuser/myrepository2.git";
        string depEx2 = "com.mycompany.mypackage1\": \"https://github.example.com/myuser/myrepository1.git";

        GUILayout.Label("Example of dependencie: " + depEx1);
        GUILayout.Label("Example of dependencie: " + depEx2);
        if (GUILayout.Button("More examples"))
            Application.OpenURL("https://docs.unity3d.com/Manual/upm-git.html");
        if (GUILayout.Button("Info about creating package for unity"))
            Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html");
    }
    #endregion
    void CreateFiles(string path)
    {
        path += "/" + _name;
        Directory.CreateDirectory(path);
        path += "/";
        //=======================================================================================================
        string fileName = "package.json";
        if (!File.Exists(path + fileName))
        {
            string depend = "";
            for (int i = 0; i < dependencies.Count; i++)
            {
                depend += "    ";
                if (i != 0)
                    depend += "\n";
                depend += dependencies[i];
            }
            string data =
                "{\n"
                + $"\"name\": \"{_name}\",\n"
                + $"\"displayName\": \"{displName}\",\n"
                + $"\"version\": \"{version}\",\n"
                + $"\"unity\": \"{unityVers}\",\n"
                + $"\"description\": \"{descr}\",\n"
                + $"\"author\": \"{author}\",\n"
                + (category != "" ? 
                  $"\"category\": \"{category}\",\n" : "")

                + (repoData.url != "" ? //Repo---------
                "\"repository\": \n"
                + "{\n"
                + $"  \"type\": \"{repoData.type}\",\n"
                + $"  \"url\": \"{repoData.url}\",\n"
                + "}\n"
                : "") //-------------------------------

                + (dependencies != null && dependencies.Count > 0 ? //dependencies---------
                "\"dependencies\": \n"
                + "{\n" +
                        dependencies
                + "}\n"
                : "")//---------------------------------------------------------------------
                + "}\n"
                ;
        File.WriteAllText(path + fileName, data);
        }
        //=======================================================================================================
        fileName = "README.md";
        //File.Create(path + fileName); //Выдает ошибку чтения!!
        if (!File.Exists(path + fileName))
            File.WriteAllText(path + fileName, "");
        //=======================================================================================================

      //  string tempFolderName = "";
      //  string fullFilePath;

        if (folder_Editor)
            CreateDirectoryWithFile_ASMDEF(path + "Editor", "Editor", folder_Runtime ? "Runtime" : "", true);

        if (folder_Runtime) 
            CreateDirectoryWithFile_ASMDEF(path + "Runtime", "Runtime");

        if (folder_TestEditor || folder_TestRuntime)
        {
            if (!Directory.Exists(path + "Test"))
                Directory.CreateDirectory(path + "Test");
            if (folder_TestEditor)
                CreateDirectoryWithFile_ASMDEF(path + "Test/Editor", "EditorTests");

            if (folder_TestRuntime)
                CreateDirectoryWithFile_ASMDEF(path + "Test/Runtime", "RuntimeTests");
        }
  
        if(folder_Samples) Directory.CreateDirectory(path + "Samples");
        if(folder_Documentation) Directory.CreateDirectory(path + "Documentation");

        AssetDatabase.Refresh();
    }


    void CreateDirectoryWithFile_ASMDEF(string path, string targetName, string referencePostfixASMDEF = "", bool isOnlyEditorPlatform = false)
    {
        string fullFilePath = $"{path}/{_name}.{targetName}.asmdef";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(fullFilePath))
            File.WriteAllText(fullFilePath, GenerateASMDEF(targetName, referencePostfixASMDEF, isOnlyEditorPlatform)); //.asmdef
    }
        

    string GenerateASMDEF(string postfix, string referencesPostfix, bool isOnlyEditorPlatform)
    {
        return "{\n"
                + $"\"name\" : \"{_name}.{postfix}\""
                + (referencesPostfix != "" ?
                $",\n \"references\": [\n"
                + $"     \"{_name}.{referencesPostfix}\"\n"
                + $" ],\n"
                + $" \"optionalUnityReferences\": [],\n"

                + (isOnlyEditorPlatform ?
                $" \"includePlatforms\": [\n"
                + $"     \"Editor\"\n"
                + $" ],\n"
                : "")

                + $" \"excludePlatforms\": [],\n"
                + $" \"allowUnsafeCode\": false,\n"
                + $" \"overrideReferences\": false,\n"
                + $" \"precompiledReferences\": [],\n"
                + $" \"autoReferenced\": true,\n"
                + $" \"defineConstraints\": []\n"
                : "")
                + "\n}"
                ;
    }

}
