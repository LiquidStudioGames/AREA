using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct AssetObject : INetworkObject
{
    public string path;
    public string bundle;

#if UNITY_EDITOR
    [HideInInspector]
    public UnityEngine.Object asset;
#endif

    public AssetObject Copy => new AssetObject { path = path, bundle = bundle };

    public AssetObject(string path, string bundle)
    {
        this.path = path;
        this.bundle = bundle;
#if UNITY_EDITOR
        asset = null;
#endif
    }
#if UNITY_EDITOR
    public AssetObject(UnityEngine.Object asset)
    {
        this.asset = asset;
        path = AssetDatabase.GetAssetPath(asset);
        bundle = AssetImporter.GetAtPath(path).assetBundleName;
    }
#endif

    public void Serialize(BitStream stream)
    {
        stream.Write(path);
        stream.Write(bundle);
    }

    public void Deserialize(BitStream stream)
    {
        path = stream.ReadString();
        bundle = stream.ReadString();
    }

    public T Load<T>() where T : UnityEngine.Object
    {
        return AssetBundleLoader.GetAssetFromBundle<T>(this);
    }
}

public static class AssetBundleLoader
{
    public static Dictionary<Tuple<string, string>, UnityEngine.Object> cache = new Dictionary<Tuple<string, string>, UnityEngine.Object>();
#if UNITY_EDITOR

    public static void LoadAssetBundle(string bundle) { }
    private static void LoadDependencies(string bundle) { }
    private static void UnloadDependencies(string bundle, bool keepObjects) { }

    public static T GetAssetFromBundle<T>(AssetObject asset) where T : UnityEngine.Object
    {
        string bundle = AssetImporter.GetAtPath(asset.path).assetBundleName;

        if (string.IsNullOrWhiteSpace(bundle))
        {
            Debug.LogError($"Asset at {asset.path} is not in an AssetBundle.");
            return null;
        }

        if (bundle != asset.bundle)
        {
            Debug.LogError($"Bundle of AssetObject at {asset.path} is not correct. {asset.bundle} =/= {bundle}");
            return null;
        }

        if (!Game.Instance.IsClient)
        {
            asset.path = asset.path.Replace("Assets/", "Assets/Stripped_Assets/");
        }

        Tuple<string, string> key = Tuple.Create(asset.bundle, asset.path);

        if (!cache.ContainsKey(key))
        {
            cache.Add(key, AssetDatabase.LoadAssetAtPath<T>(asset.path));
        }

        return cache[key] as T;
    }

    public static void DisposeAssetBundle(string bundle, bool keepObjects = true, bool unloadDependencies = true)
    {
        foreach (Tuple<string, string> key in cache.Keys.Where(x => x.Item1 == bundle))
            cache.Remove(key);
    }

#else
    private static AssetBundleManifest manifest = null;
    private static Dictionary<string, AssetBundle> assetbundles = new Dictionary<string, AssetBundle>();

    public static void LoadAssetBundle(string bundle)
    {
        string path = Application.dataPath + "/AssetBundles/" + bundle.ToLower();

        if (!Game.Instance.IsClient)
        {
            if (!bundle.Contains("stripped_")) bundle = "stripped_" + bundle;
            path = Application.dataPath + "/Stripped_AssetBundles/" + bundle.ToLower();
        }

        if (assetbundles.ContainsKey(bundle)) return;
        AssetBundle b = AssetBundle.LoadFromFile(path);
        assetbundles.Add(bundle, b);
        LoadDependencies(b.name);
    }

    public static T GetAssetFromBundle<T>(AssetObject asset) where T : UnityEngine.Object
    {
        if (!Game.Instance.IsClient)
        {
            string ex = System.IO.Path.GetExtension(asset.path);
            if (ex != ".prefab" && ex != ".asset") return null;

            if (!asset.bundle.Contains("stripped_")) asset.bundle = "stripped_" + asset.bundle;
            asset.path = asset.path.Replace("Assets/", "Assets/Stripped_Assets/");
        }

        if (!assetbundles.ContainsKey(asset.bundle)) LoadAssetBundle(asset.bundle);
        Tuple<string, string> key = Tuple.Create(asset.bundle, asset.path);

        if (!cache.ContainsKey(key))
        {
            cache.Add(key, assetbundles[asset.bundle].LoadAsset<T>(asset.path));
        }

        if (cache[key] == null)
        {
            Debug.LogError($"AssetObject for {asset.path} returned null.");
        }

        return cache[key] as T;
    }

    public static void DisposeAssetBundle(string bundle, bool keepObjects = true, bool unloadDependencies = true)
    {
        if (!Game.Instance.IsClient)
        {
            if (!bundle.Contains("stripped_")) bundle = "stripped_" + bundle;
        }

        if (assetbundles.ContainsKey(bundle))
        {
            assetbundles[bundle].Unload(!keepObjects);
            assetbundles.Remove(bundle);

            foreach (Tuple<string, string> key in cache.Keys.Where(x => x.Item1 == bundle))
                cache.Remove(key);

            if (unloadDependencies)
            {
                UnloadDependencies(bundle, keepObjects);
            }
        }
    }

    private static void LoadDependencies(string bundle)
    {
        if (manifest == null)
        {
            string assetBundlesPath = "/AssetBundles/AssetBundles";

            if (!Game.Instance.IsClient)
            {
                assetBundlesPath = "/Stripped_AssetBundles/Stripped_AssetBundles";
            }

            manifest = AssetBundle.LoadFromFile(Application.dataPath + assetBundlesPath).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null) throw new NullReferenceException("Assetbundlemanifest is not found. {" + assetBundlesPath + "}");
        }

        foreach (string d in manifest.GetAllDependencies(bundle))
        {
            if (!assetbundles.ContainsKey(d)) LoadAssetBundle(d);
        }
    }

    private static void UnloadDependencies(string bundle, bool keepObjects)
    {
        if (manifest == null)
        {
            string assetBundlesPath = "/AssetBundles/AssetBundles";

            if (!Game.Instance.IsClient)
            {
                assetBundlesPath = "/Stripped_AssetBundles/Stripped_AssetBundles";
            }

            manifest = AssetBundle.LoadFromFile(Application.dataPath + assetBundlesPath).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null) throw new NullReferenceException("Assetbundlemanifest is not found. {" + assetBundlesPath + "}");
        }

        foreach (string d in manifest.GetAllDependencies(bundle))
        {
            DisposeAssetBundle(d, keepObjects);
        }
    }
#endif
}