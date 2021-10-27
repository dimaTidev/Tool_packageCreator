using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoPackageCreator_Window : EditorWindow
{
    static SerializedObject serializedObject;

    public PackageJson packageJson = new PackageJson();

    #region jsons
    //--------------------------------------------------------------------------------------
    public class PackageJson
    {
        public string name = "com.accName.repoName",
                    displayName = "Test repo",
                    version = "0.0.1",
                    unity = "2018.4",
                    description = "",
                    author = "DimaTi <timofeenkodima@gmail.com> (https://github.com/dimaTidev)",
                   // keywords,
                    category;
   
        public Repository repository;
        public List<string> dependencies = null;
    }

    public class AsmdefJson
    {
        public string name;
        public List<string> references;
        public List<string> optionalUnityReferences;
        public List<string> includePlatforms;
        public List<string> excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public List<string> precompiledReferences;
        public bool autoReferenced;
        public List<string> defineConstraints;
    }
    //--------------------------------------------------------------------------------------
    #endregion

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
    public class Repository
    {
        public string 
            type = "git", 
            url = "https://github.com/dimaTidev/Package_MethodsReflection.git";
    }
    public Repository repoData;
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
        OnGUI_DrawAsmdef();

        Draw_SaveFolder();

        GUI.enabled = saveFolder;

        EditorGUILayout.BeginHorizontal(); //--------

        EditorGUILayout.BeginVertical("Helpbox"); //~~~

        packageJson.name = EditorGUILayout.TextField("name", packageJson.name);

        bool isAlreadyHasPackage = false;

        if (saveFolder)
        {
            string dirPath = AssetDatabase.GetAssetPath(saveFolder);

          // string[] packages = Directory.GetFiles(dirPath, "*.json");
          // if (packages != null && packages.Length > 0)
          // {
          //     isAlreadyHasPackage = true;
          //     packageJson = JsonUtility.FromJson<PackageJson>(File.ReadAllText(packages[0]));
          // }

            if (!isAlreadyHasPackage)
            {
                string path = dirPath + "/" + packageJson.name;
                if (Directory.Exists(path))
                {
                    path += "/" + "package.json";
                    if (File.Exists(path))
                    {
                        EditorGUILayout.LabelField("folder already contain package.json!");
                        GUI.enabled = false;
                        // isAlreadyHasPackage = true;
                    }
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

    #region ASMDEF
    //--------------------------------------------------------------------------------------------
    TextAsset selectedASMDEF;

    void OnGUI_DrawAsmdef()
    {
        selectedASMDEF = EditorGUILayout.ObjectField(selectedASMDEF, typeof(TextAsset), true) as TextAsset;
        if(selectedASMDEF)
            if (GUILayout.Button("check"))
            {
                AsmdefJson json = JsonUtility.FromJson<AsmdefJson>(selectedASMDEF.text);

                foreach (var item in json.references)
                {
                    Debug.Log("references: " + item);
                }
            }
    }

    void FindPackajeJson(string path)
    {
        Debug.Log("Application.dataPath:" + Application.dataPath);

       // while (path != Application.dataPath)
       // {
       //
       // }
       //
       // DirectoryInfo d = new DirectoryInfo(path); //Assuming Test is your Folder
       //
       // FileInfo[] files = d.GetFiles("*.json"); //Getting Text files
       // 
       // if (files != null && files.Length > 0)
       // {
       //
       // }
    }
    //--------------------------------------------------------------------------------------------
    #endregion

    #region Drawers
    //------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DrawLines()
    {
        packageJson.displayName = EditorGUILayout.TextField("displayName", packageJson.displayName);
        packageJson.version = EditorGUILayout.TextField("version", packageJson.version);
        packageJson.unity = EditorGUILayout.TextField("unity", packageJson.unity);
        packageJson.author = EditorGUILayout.TextField("author", packageJson.author);
        packageJson.category = EditorGUILayout.TextField("category", packageJson.category);
        // EditorGUILayout.TextArea("description", packageJson.descr);
        EditorGUILayout.LabelField("description");
        packageJson.description = EditorGUILayout.TextArea(packageJson.description);
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
    //------------------------------------------------------------------------------------------------------------------------------------------------------------
    #endregion
    void CreateFiles(string path)
    {
        packageJson.repository = repoData;
        packageJson.dependencies = dependencies;

        path += "/" + packageJson.name;
        Directory.CreateDirectory(path);
        path += "/";
        //=======================================================================================================
        string fileName = "package.json";
        if (!File.Exists(path + fileName))
        {
            string depend = "";
            for (int i = 0; i < packageJson.dependencies.Count; i++)
            {
                depend += "    ";
                if (i != 0)
                    depend += "\n";
                depend += packageJson.dependencies[i];
            }
            string data = JsonUtility.ToJson(packageJson, true);
            //    "{\n"
            //    + $"\"name\": \"{packageJson._name}\",\n"
            //    + $"\"displayName\": \"{packageJson.displName}\",\n"
            //    + $"\"version\": \"{packageJson.version}\",\n"
            //    + $"\"unity\": \"{packageJson.unityVers}\",\n"
            //    + $"\"description\": \"{packageJson.descr}\",\n"
            //    + $"\"author\": \"{packageJson.author}\",\n"
            //    + (packageJson.category != "" ? 
            //      $"\"category\": \"{packageJson.category}\",\n" : "")
            //
            //    + (packageJson.repository.url != "" ? //Repo---------
            //    "\"repository\": \n"
            //    + "{\n"
            //    + $"  \"type\": \"{packageJson.repository.type}\",\n"
            //    + $"  \"url\": \"{packageJson.repository.url}\",\n"
            //    + "}\n"
            //    : "") //-------------------------------
            //
            //    + (depend != "" ? //dependencies---------
            //    "\"dependencies\": \n"
            //    + "{\n" +
            //            depend
            //    + "}\n"
            //    : "")//---------------------------------------------------------------------
            //    + "}\n"
            //    ;
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
                CreateDirectoryWithFile_ASMDEF(path + "Test/Runtime", "RuntimeTests", folder_Runtime ? "Runtime" : "");
        }
  
        if(folder_Samples) Directory.CreateDirectory(path + "Samples");
        if(folder_Documentation) Directory.CreateDirectory(path + "Documentation");

       // Selection.activeObject = 
        AssetDatabase.Refresh();
    }
    void CreateDirectoryWithFile_ASMDEF(string path, string targetName, string referencePostfixASMDEF = "", bool isOnlyEditorPlatform = false)
    {
        string fullFilePath = $"{path}/{packageJson.name}.{targetName}.asmdef";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(fullFilePath))
            File.WriteAllText(fullFilePath, GenerateASMDEF(targetName, referencePostfixASMDEF, isOnlyEditorPlatform)); //.asmdef
    }
    string GenerateASMDEF(string postfix, string referencesPostfix, bool isOnlyEditorPlatform)
    {
        return "{\n"
                + $"\"name\" : \"{packageJson.name}.{postfix}\""
                + (referencesPostfix != "" ?
                $",\n \"references\": [\n"
                + $"     \"{packageJson.name}.{referencesPostfix}\"\n"
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
