using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PackageCreator
{
    [System.Serializable]
    public class PackageJson
    {
        public string
            name,
            displayName,
            version;

      [EditorUnityVersion(true)]  
        public string unity;
      [EditorUnityVersion(false)] public string unityRelease;

        [TextArea]
        public string description;

        public string author;

       [StringSelector(new string[]{
                    "Base",
                    "Nested",
                    "Shaders",
                    "Editor"
                }, true, false)] public List<string> keywords;

        [StringSelector(new string[]{
                    "Base",
                    "Nested",
                    "Shaders",
                    "Editor"
                }, true)]
        public string category;

        public Repository repository;
        // public List<string> dependencies = null;

        [NonSerialized] public Dictionary<string, string> gitDependencies; //Unity JsonUtility didn't serialize Dictionaries. And we do it by hand.
        [NonSerialized] public Dictionary<string, string> dependencies; //Unity JsonUtility didn't serialize Dictionaries. And we do it by hand.

        public PackageJson(bool isUseTemp = true)
        {
            name = "com.accName.repoName";
            displayName = "Test repo";
            version = "0.0.1";

            unity = "2018.4";
           // unityRelease = "27f1";

            description = "";
            author = "DimaTi <timofeenkodima@gmail.com> (https://github.com/dimaTidev)";
            repository = new Repository("https://github.com/dimaTidev/Tool_packageCreator.git");
        }


        [System.Serializable]
        public class Repository
        {
            public string type;
            [Link] public string url;

            public Repository(string url, string type = "git")
            {
                this.type = type;
                this.url = url;
            }

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

        public static PackageJson Parce(string value)
        {
            if (value == "")
                return null;

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
}
