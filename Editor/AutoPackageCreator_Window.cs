using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace PackageCreator
{
    public class AutoPackageCreator_Window : EditorWindow
    {
       // static SerializedObject serializedObject;

        public PackageJson packageJson = new PackageJson();

        #region jsons
        //--------------------------------------------------------------------------------------
        [System.Serializable]
        public class PackageJson
        {
            public string name = "com.accName.repoName",
                        displayName = "Test repo",
                        version = "0.0.1",
                        unity = "2018.4";
            [TextArea]
            public string description = "";

            public string author = "DimaTi <timofeenkodima@gmail.com> (https://github.com/dimaTidev)",
                       // keywords,
                        category;
   
            public Repository repository;
            // public List<string> dependencies = null;

            [NonSerialized] public Dictionary<string, string> gitDependencies; //Unity JsonUtility didn't serialize Dictionaries. And we do it by hand.

            [NonSerialized] public Dictionary<string, string> dependencies; //Unity JsonUtility didn't serialize Dictionaries. And we do it by hand.


            public static PackageJson Parce(string value)
            {
                PackageJson package = JsonUtility.FromJson<PackageJson>(value);
                package.DeserializeJsonForDictionary(value);
              //  package.repository = new Repository(value);
                return package; 
            }

            public string SerializeDictionaryToJson(string value)
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                list.Add(gitDependencies);
                list.Add(dependencies);

                List<string> depsNames = new List<string>()
                {
                    "gitDependencies",
                    "dependencies"
                };

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == null)
                        continue;

                    if (value.EndsWith("}"))
                        value = value.Remove(value.Length - 2);
                    if (!value.EndsWith(","))
                        value += ",";
                    value += "\n";

                    value += SerialezeDict(list[i], depsNames[i]);
                    value += "\n}";
                }
                return value;
            }
            public void DeserializeJsonForDictionary(string value)
            {
                gitDependencies = new Dictionary<string, string>();
                dependencies = new Dictionary<string, string>();

                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                list.Add(gitDependencies);
                list.Add(dependencies);

                List<string> depsNames = new List<string>()
                {
                    "gitDependencies",
                    "dependencies"
                };

                for (int i = 0; i < list.Count; i++)
                {
                    if (value.Contains(depsNames[i]))
                    {
                        int idStart = value.IndexOf(depsNames[i]);
                        int idEnd = value.IndexOf("}", idStart);

                        if (idStart < 0)
                            Debug.Log("idStart < 0 : " + idStart);
                        if (idEnd < 0)
                            Debug.Log("idEnd < 0 : " + idEnd);

                        if (idStart < 0 || idEnd < 0)
                            return;

                        DeserializeDict(list[i], value.Substring(idStart, idEnd - idStart));
                    }
                }
            }

            string SerialezeDict(Dictionary<string, string> gitDependencies, string depsLabel)
            {
                string result = $"    \"{depsLabel}\": {{";
                foreach (var item in gitDependencies)
                    result += $"\n        \"{item.Key}\": \"{item.Value}\",";

                if (result.EndsWith(","))
                    result = result.Remove(result.LastIndexOf(",")); //remove coma

                result += "\n    }";
                return result;
            }

            void DeserializeDict(Dictionary<string, string> dict, string value)
            {
                int idStart = value.IndexOf("{");
                int idEnd = idStart;

                int recCount = 0;
                while (true)
                {
                    idStart = value.IndexOf("\"", idEnd + 1); //find first com.company.package
                    idEnd = value.IndexOf("\"", idStart + 1);

                    if (idStart >= idEnd || idStart < 0) //if we come to end of text
                        break;

                    //Debug.Log("idStart : " + idStart + " idEnd:" + idEnd);
                    string part0 = value.Substring(idStart + 1, idEnd - idStart - 1);

                    idStart = value.IndexOf("\"", idEnd + 1);
                    idEnd = value.IndexOf("\"", idStart + 1);

                    string part1 = value.Substring(idStart + 1, idEnd - idStart - 1);

                    dict.Add(part0, part1);

                    recCount++;
                    if (recCount > 100)
                    {
                        Debug.LogError("Recurcie > 100 : " + idStart + " idEnd:" + idEnd);
                        break;
                    }
                }
            }
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
        bool isExistPackageInFolder;

        bool
            folder_Editor,
            folder_Runtime = true,
            folder_TestEditor,
            folder_TestRuntime,
            folder_Samples,
            folder_Documentation;

        bool
            foldoutGitDepencies,
            foldoutGitDepenciesNew;

        bool foldoutExamples;


        [System.Serializable]
        public class Repository
        {
            public string 
                type = "git", 
                url = "https://github.com/dimaTidev/Tool_packageCreator.git";

           // public Repository(string value)
           // {
           //     if (value.Contains("\"repository\":"))
           //     {
           //         int idStart = value.IndexOf("{", value.IndexOf("\"repository\":"));
           //         int idEnd = value.IndexOf("}", idStart);
           //         string substring = value.Substring(idStart, idEnd - idStart + 1);
           //
           //         Repository repo = JsonUtility.FromJson<Repository>(substring);
           //
           //         type = repo.type;
           //         url = repo.url;
           //     }
           // }
        }
       // [AssetMenu]
        [MenuItem("Tools/Package Creator")]
        static void Init()
        {
            AutoPackageCreator_Window window = (AutoPackageCreator_Window)EditorWindow.GetWindow(typeof(AutoPackageCreator_Window));
            window.Show();
        }

        void OnGUI()
        {
          // if (GUILayout.Button("TestSave"))
          //     TestSave();

            EditorGUI.BeginChangeCheck();
            Draw_SaveFolder();
            if (EditorGUI.EndChangeCheck())
                isExistPackageInFolder = CheckSaveFolder(saveFolder);

            GUI.enabled = saveFolder;

            EditorGUILayout.BeginHorizontal(); //--------
            EditorGUILayout.BeginVertical("Helpbox"); //~~~

           // packageJson.name = EditorGUILayout.TextField("name", packageJson.name);

             //DrawLines(packageJson);
             Draw_Repo();
             OnGUI_FindedDependencies();
             //   Draw_Dependencies();
             EditorGUILayout.EndVertical(); //~~~
        
             GUI.enabled = saveFolder;
        
             EditorGUILayout.BeginVertical("Helpbox", GUILayout.MaxWidth(120)); //~~~
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
            // GUI.enabled = isAlreadyHasPackage;
            if (GUILayout.Button(isExistPackageInFolder ? "apply changes" : "Create hierarchy"))
                CreateFiles(AssetDatabase.GetAssetPath(saveFolder));
        
             GUI.color = Color.white;
             GUI.enabled = true;
        }

        bool CheckSaveFolder(UnityEngine.Object saveFolder)
        {
            bool isExistPackage = false;
            if (saveFolder)
            {
                string dirPath = AssetDatabase.GetAssetPath(saveFolder);

                string[] packages = Directory.GetFiles(dirPath, "package.json");
                if (packages != null && packages.Length > 0)
                {
                    isExistPackage = true;
                    packageJson = PackageJson.Parce(File.ReadAllText(packages[0]));
                }

                if (!isExistPackage)
                {
                    string path = dirPath + "/" + packageJson.name;
                    if (Directory.Exists(path))
                    {
                        path += "/" + "package.json";
                        if (File.Exists(path))
                        {
                            EditorGUILayout.LabelField("folder already contain package.json!");
                            isExistPackage = true;
                            packageJson = PackageJson.Parce(File.ReadAllText(path));
                        }
                    }
                }

                if (isExistPackage)
                {
                    Search_newDependenciesInWholeProject();
                    FolderCheck(dirPath);
                }
                    
            }

            if (!isExistPackage)
                packageJson = new PackageJson();
        
            return isExistPackage;
        }

        void FolderCheck(string path)
        {
            //TODO: add check folders
            if (!path.EndsWith("/"))
                path += "/";

            folder_Editor = Directory.Exists(path + "Editor");
            folder_Runtime = Directory.Exists(path + "Runtime");
            folder_TestEditor = Directory.Exists(path + "Test") && Directory.Exists(path + "Test/Editor");
            folder_TestRuntime = Directory.Exists(path + "Test") && Directory.Exists(path + "Test/Runtime");
            folder_Samples = Directory.Exists(path + "Samples");
            folder_Documentation = Directory.Exists(path + "Documentation");
        }

        #region ASMDEF
        //--------------------------------------------------------------------------------------------
        void Search_newDependenciesInWholeProject()
        {
            //selectedASMDEF = EditorGUILayout.ObjectField(selectedASMDEF, typeof(TextAsset), true) as TextAsset;
            if (!saveFolder)
                return;
              // if (GUILayout.Button("check"))
              // {
                    List<string> asmdefPaths = new List<string>();
                    string path = AssetDatabase.GetAssetPath(saveFolder);
                    string[] files = Directory.GetFiles(path, "*.asmdef", SearchOption.AllDirectories);
                    asmdefPaths.AddRange(files);

                    List<string> usedAsmdefs = new List<string>();
                    foreach (var pathA in asmdefPaths)
                    {
                        AsmdefJson json = JsonUtility.FromJson<AsmdefJson>(File.ReadAllText(pathA));
                        usedAsmdefs.AddRange(json.references);
                    }
                    // AsmdefJson json = JsonUtility.FromJson<AsmdefJson>(selectedASMDEF.text);
                    findedDeps = FindAllASMDEFinAssets(usedAsmdefs);

                    files = Directory.GetFiles(path, "package.json", SearchOption.AllDirectories);
                    foreach (var packagePath in files)
                    {
                        PackageJson package = JsonUtility.FromJson<PackageJson>(File.ReadAllText(packagePath));
                        if (findedDeps.ContainsKey(package.name))
                            findedDeps.Remove(package.name);
                    }

                   // foreach (var item in findedDeps)
                   //     Debug.Log($"item[{item.Value.isInAssetFolder}]: " + item.Key + "  " + item.Value.url);
              //  }
        }

        public void CallbackDepChange<K>(Dictionary<string, K> dep, string key)
        {
           if (dep[key] is URLVersion urlData)
           {
             //  string value = urlData.url;
               bool isUnityPackage = key.Contains("com.unity.");

               if (packageJson.gitDependencies == null) packageJson.gitDependencies = new Dictionary<string, string>();
               if (packageJson.dependencies == null) packageJson.dependencies = new Dictionary<string, string>();
          
               Dictionary<string, string> dict = isUnityPackage ? packageJson.dependencies : packageJson.gitDependencies;
              // if (packageJson.dependencies == null)
              //     packageJson.dependencies = new Dictionary<string, string>();
               if (dict.ContainsKey(key))
                   dict.Remove(key);
               dict.Add(key, isUnityPackage ? urlData.version : urlData.UrlWithVersion);
           }

           // if (dep[key] is URLVersion urlData)
           // {
           //     string value = urlData.url;
           //     if (packageJson.dependencies == null)
           //         packageJson.dependencies = new Dictionary<string, string>();
           //     if (packageJson.dependencies.ContainsKey(key))
           //         packageJson.dependencies.Remove(key);
           //     packageJson.dependencies.Add(key, value);
           // }
        }

        public void CallbackGitDepChange<K>(Dictionary<string, K> dep, string key)
        {
            if (dep[key] is URLVersion urlData)
            {
                string value = urlData.url;
                if (packageJson.gitDependencies == null)
                    packageJson.gitDependencies = new Dictionary<string, string>();
                if (packageJson.gitDependencies.ContainsKey(key))
                    packageJson.gitDependencies.Remove(key);
                packageJson.gitDependencies.Add(key, value);
            }
        }

        void OnGUI_FindedDependencies()
        {
            OnGUI_DrawDictionary(ref findedDeps, "GitDependencies Search", ref foldoutGitDepenciesNew, false, CallbackDepChange, null, "add"); //CallbackDepChange, "git", "uni"
        }

        Dictionary<string, URLVersion> findedDeps;

        public class URLVersion
        {
            public string url; 
            public string version; 
            public bool isInAssetFolder;

            public string UrlWithVersion => url + "#" + version;

            public URLVersion(string url, string version, string fullPath)
            {
                this.url = url;
                this.version = version;
                if (fullPath.Contains("Assets"))
                    isInAssetFolder = true;
            }

            public override string ToString() => UrlWithVersion + (isInAssetFolder ? "  ASSET FOLDER" : "");
        }

        Dictionary<string, URLVersion> FindAllASMDEFinAssets(List<string> asmdefNames)
        {
            List<string> packages = new List<string>();

            string path = Application.dataPath; //search in asset directory
            string[] files = Directory.GetFiles(path, "package.json", SearchOption.AllDirectories);
            packages.AddRange(files);

            path = Path.GetFullPath(Path.Combine(path, @"..\")) + @"Library\PackageCache\"; //search in package directory
            files = Directory.GetFiles(path, "package.json", SearchOption.AllDirectories);
            packages.AddRange(files);

            List<string> usedPackages = new List<string>();

            foreach (var packagePath in packages)
            {
                 string[] asmdefs = Directory.GetFiles(Path.GetDirectoryName(packagePath), "*.asmdef", SearchOption.AllDirectories);
                 foreach (var asmdefPath in asmdefs)
                 {
                    AsmdefJson json = JsonUtility.FromJson<AsmdefJson>(File.ReadAllText(asmdefPath));
                    if (asmdefNames.Contains(json.name))
                         usedPackages.Add(packagePath);
                 }
            }

            Dictionary<string, URLVersion> dict = new Dictionary<string, URLVersion>();

            foreach (var packagePath in usedPackages)
            {
                PackageJson packageJson = JsonUtility.FromJson<PackageJson>(File.ReadAllText(packagePath));
                if(!dict.ContainsKey(packageJson.name))
                    dict.Add(packageJson.name, new URLVersion(packageJson.repository.url, packageJson.version, packagePath));
            }

            return dict;
        }

        List<string> ClearPath(string[] files)
        {
            List<string> deps = new List<string>();

            foreach (var item in files)
                deps.Add(Path.GetFileName(item));

            return deps;
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
        
            SerializedProperty list = so.FindProperty("packageJson");
            EditorGUILayout.PropertyField(list, true);
            so.ApplyModifiedProperties();

            OnGUI_DrawDictionary(ref packageJson.gitDependencies, "git Dependencies", ref foldoutGitDepencies, true);
            OnGUI_DrawDictionary(ref packageJson.dependencies, "dependencies", ref foldoutGitDepencies, true);
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

        #region Creation
        void CreateFiles(string path)
        {
           // packageJson.repository = repoData;
            //   packageJson.dependencies = dependencies;

            if (!isExistPackageInFolder)
            {
                path += "/" + packageJson.name;
                Directory.CreateDirectory(path);
            }
       
            path += "/";
            //=======================================================================================================
            string fileName = "package.json";
           // if (!File.Exists(path + fileName))
           // {
               /* string depend = "";
                for (int i = 0; i < packageJson.dependencies.Count; i++)
                {
                    depend += "    ";
                    if (i != 0)
                        depend += "\n";
                    depend += packageJson.dependencies[i];
                }*/
            string data = JsonUtility.ToJson(packageJson, true);
            data = packageJson.SerializeDictionaryToJson(data);

            File.WriteAllText(path + fileName, data);
           // }
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
  
            if(folder_Samples && !Directory.Exists(path + "Samples~"))  Directory.CreateDirectory(path + "Samples~");
            if(folder_Documentation && !Directory.Exists(path + "Documentation~")) Directory.CreateDirectory(path + "Documentation~");

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
        #endregion

        //-------------------------------------------------------------------------------
        public void OnGUI_DrawDictionary<T, K>(
          ref Dictionary<T, K> dictionary, string label, ref bool foldout, bool isCanDelete = false,
          Action<Dictionary<T, K>, T> callbackButtonAction = null, Action<Dictionary<T, K>, T> callbackButtonAction2 = null,
          string labelButt0 = "0", string labelButt1 = "1")
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                EditorGUILayout.LabelField($"{label} - Null or zero");
                return;
            }

            foldout = EditorGUILayout.Foldout(foldout, label);
            if (!foldout)
                return;

            List<T> key_forDelete = new List<T>();

            EditorGUILayout.BeginVertical("Helpbox");
            foreach (var item in dictionary)
            {
                EditorGUILayout.BeginHorizontal();
                if (isCanDelete)
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button("x", GUILayout.Width(20))) //delete
                        key_forDelete.Add(item.Key);
                }

                GUI.color = Color.white;
                if (callbackButtonAction != null)
                {
                    GUI.color = Color.white;
                    if (GUILayout.Button(labelButt0, GUILayout.Width(35)))
                        callbackButtonAction?.Invoke(dictionary, item.Key);
                }
                GUI.color = Color.white;
                if (callbackButtonAction2 != null)
                {
                    GUI.color = Color.white;
                    if (GUILayout.Button(labelButt1, GUILayout.Width(35)))
                        callbackButtonAction2?.Invoke(dictionary, item.Key);
                }

                GUI.color = Color.white;

                EditorGUILayout.LabelField(item.Key.ToString(), GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField(item.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }

            foreach (var item in key_forDelete)
                dictionary.Remove(item);

            EditorGUILayout.EndVertical();
        }
    }
}
