using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mod
{
    public string Name { get; set; }
}

public class ModEditor : EditorWindow
{
    public string mod = "New Mod";
    public string path = "Mod/";
    public bool customScripts = true;
    
    [MenuItem("Mod Editor/Open")]
    static void Init()
    {
        ModEditor window = (ModEditor)GetWindow(typeof(ModEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Mod Settings", EditorStyles.boldLabel);
        mod = EditorGUILayout.TextField("Mod", mod);
        path = EditorGUILayout.TextField("Build Path", path);
        customScripts = GUILayout.Toggle(customScripts, "Custom Scripts");

        if (GUILayout.Button("Build Mod"))
        {
            Build();
        }
    }

    public void Build()
    {
        if (Directory.Exists(path)) Directory.Delete(path, true);
        Directory.CreateDirectory(path);

        if (customScripts)
        {
            BuildWithScripts();
        }

        else
        {
            Debug.Log("Exporting Assetbundles");
            if (Directory.Exists(path + "AssetBundles")) Directory.Delete(path + "AssetBundles", true);
            Directory.CreateDirectory(path + "AssetBundles");
            BuildPipeline.BuildAssetBundles(path + "/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        Debug.Log("Zipping");
        File.WriteAllText(path + mod + ".json", JsonConvert.SerializeObject(new Mod { Name = mod }));
        
        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
            zip.Save(path + mod + ".zip");
        }
    }

    public void BuildWithScripts()
    {
        if (Directory.Exists("Assets/Converted")) Directory.Delete("Assets/Converted", true);
        Directory.CreateDirectory("Assets/Converted");

        // Convert all components
        Debug.Log("Converting Prefabs");
        StripComponents();

        Debug.Log("Converting Scenes");
        StripScenes();

        // Build assetbundles from Assets/Converted
        Debug.Log("Exporting Assetbundles");
        string[] assets = AssetDatabase.FindAssets("t: Object", new string[1] { "Assets/Converted" }).Where(x => !string.IsNullOrEmpty(AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(x)).assetBundleName)).ToArray();
        BuildBundles(assets, path + "AssetBundles", BuildTarget.StandaloneWindows64);

        // Export dlls
        Debug.Log("Exporting Assemblies");
        Directory.CreateDirectory(path + "Assemblies");
        UnityEditor.Compilation.Assembly[] playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);

        foreach (var assembly in playerAssemblies)
        {
            string a = assembly.outputPath;
            string b = Path.GetFileName(a);
            if (File.Exists(a)) File.Copy(a, path + "Assemblies/" + b);
        }
    }

    private void StripComponents()
    {
        foreach (string assetid in AssetDatabase.FindAssets("t:Object"))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetid);
            string bundle = AssetImporter.GetAtPath(assetPath).assetBundleName;
            string strippedAssetPath = assetPath.Replace("Assets/", "Assets/Converted/");

            if (string.IsNullOrEmpty(bundle))
            {
                continue;
            }

            switch (Path.GetExtension(strippedAssetPath))
            {
                case ".prefab":
                    if (!Directory.Exists(Path.GetDirectoryName(strippedAssetPath))) Directory.CreateDirectory(Path.GetDirectoryName(strippedAssetPath));
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    GameObject newObj = (GameObject)Instantiate(obj);
                    newObj.name = obj.name;

                    StripComponentsFromObject(newObj);

                    GameObject o = PrefabUtility.SaveAsPrefabAsset(newObj, strippedAssetPath);
                    if (o == null) AssetDatabase.DeleteAsset(strippedAssetPath);
                    else AssetImporter.GetAtPath(strippedAssetPath).assetBundleName = "stripped_" + bundle;
                    DestroyImmediate(newObj);
                    break;

                case ".asset":
                    if (!Directory.Exists(Path.GetDirectoryName(strippedAssetPath))) Directory.CreateDirectory(Path.GetDirectoryName(strippedAssetPath));
                    AssetDatabase.CopyAsset(assetPath, strippedAssetPath);
                    AssetImporter.GetAtPath(strippedAssetPath).assetBundleName = "stripped_" + bundle;
                    break;

                default:
                    continue;
            }

        }

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private void StripComponentsFromObject(GameObject obj)
    {
        Dictionary<GameObject, List<ReplacedComponent>> replaced = new Dictionary<GameObject, List<ReplacedComponent>>();
        Component[] components = obj.GetComponentsInChildren<Component>();

        foreach (Component c in components)
        {
            string ns = c.GetType().Namespace;

            if (ns == null || (ns != "UnityEngine" && !ns.StartsWith("UnityEngine.")))
            {
                if (!replaced.ContainsKey(c.gameObject)) replaced[c.gameObject] = new List<ReplacedComponent>();

                replaced[c.gameObject].Add(new ReplacedComponent
                {
                    type = c.GetType().AssemblyQualifiedName,
                    data = JsonConvert.SerializeObject(c, ComponentAssembler.JsonSettings)
                });

                DestroyImmediate(c);
            }
        }

        foreach (GameObject o in replaced.Keys)
        {
            o.AddComponent<ComponentAssembler>().components = replaced[o].ToList();
        }
    }

    public void StripScenes()
    {
        string path = Application.dataPath + "/Converted/Scenes";
        if (Directory.Exists(path)) Directory.Delete(path, true);
        Directory.CreateDirectory(path);

        if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(path);
        string origin = SceneManager.GetActiveScene().path;

        foreach (string scenepath in Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories))
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenepath);
            EditorSceneManager.OpenScene(scenepath, OpenSceneMode.Single);
            Debug.Log("Converting " + scenepath);

            if (EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path + "/" + sceneName + ".unity"))
            {
                Scene scene = SceneManager.GetActiveScene();

                foreach (GameObject gameobject in scene.GetRootGameObjects())
                {
                    StripComponentsFromObject(gameobject);
                }

                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path + "/" + sceneName + ".unity");
            }
        }

        if (!string.IsNullOrEmpty(origin) && origin.Contains(".unity")) EditorSceneManager.OpenScene(origin, OpenSceneMode.Single);
    }

    public static void BuildBundles(string[] assets, string path, BuildTarget target)
    {
        Debug.Log("Building AssetBundles at " + path);

        HashSet<string> processedBundles = new HashSet<string>();
        var assetBundleBuilds = GetBuildsForPaths(assets, processedBundles);

        foreach (var o in assets)
        {
            string[] paths = AssetDatabase.GetDependencies(new[] { AssetDatabase.GUIDToAssetPath(o) });
            assetBundleBuilds = assetBundleBuilds.Concat(GetBuildsForPaths(paths, processedBundles)).ToList();
        }

        FileInfo file = new FileInfo(path);

        if (file.Exists)
        {
            Debug.Log("File exists at " + path);
            Debug.Log("File: " + file.ToString());
            file.Delete();
        }

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);

        Debug.Log("Finished building AssetBundles");
    }

    private static List<AssetBundleBuild> GetBuildsForPaths(string[] assets, HashSet<string> processedBundles)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        // Get asset bundle names from selection
        foreach (var o in assets)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(o);
            var importer = AssetImporter.GetAtPath(assetPath);

            if (importer == null)
            {
                continue;
            }

            // Get asset bundle name & variant
            var assetBundleName = importer.assetBundleName;
            var assetBundleVariant = importer.assetBundleVariant;
            var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;

            // Only process assetBundleFullName once. No need to add it again.
            if (processedBundles.Contains(assetBundleFullName))
            {
                continue;
            }

            processedBundles.Add(assetBundleFullName);

            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = assetBundleName,
                assetBundleVariant = assetBundleVariant,
                assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName)
            };

            assetBundleBuilds.Add(build);
        }

        return assetBundleBuilds;
    }
}

public class MyContractResolver : DefaultContractResolver
{
    public static Type[] DontSerialize = new Type[] { typeof(MonoBehaviour), typeof(Behaviour), typeof(Component), typeof(UnityEngine.Object) };

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(x => !DontSerialize.Contains(x.DeclaringType))
                        .Select(f => base.CreateProperty(f, memberSerialization))
                        .ToList();

        props.ForEach(p => { p.Writable = true; p.Readable = true; });
        return props;
    }
}