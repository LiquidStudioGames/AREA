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

    public static void LoadExternalAssetBundle(string bundle) { }
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

    private static Dictionary<string, AssetBundleManifest> manifests = new Dictionary<string, AssetBundleManifest>();
    private static Dictionary<string, AssetBundle> assetbundles = new Dictionary<string, AssetBundle>();
    
    public static void LoadAssetBundle(string bundle, string mod = "Area")
    {
        string path;

        if (mod == "Area") path = Application.dataPath + "/AssetBundles/" + bundle.ToLower();
        else path = $"Mods/Temp/{mod}/AssetBundles/" + bundle.ToLower();

        if (assetbundles.ContainsKey(bundle)) return;
        AssetBundle b = AssetBundle.LoadFromFile(path);
        assetbundles.Add(bundle, b);
        LoadDependencies(b.name, mod);
    }

    public static T GetAssetFromBundle<T>(AssetObject asset) where T : UnityEngine.Object
    {
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

    public static void DisposeAssetBundle(string bundle, string mod = "Area", bool keepObjects = true, bool unloadDependencies = true)
    {
        if (assetbundles.ContainsKey(bundle))
        {
            assetbundles[bundle].Unload(!keepObjects);
            assetbundles.Remove(bundle);

            foreach (Tuple<string, string> key in cache.Keys.Where(x => x.Item1 == bundle))
                cache.Remove(key);

            if (unloadDependencies)
            {
                UnloadDependencies(bundle, mod, keepObjects);
            }
        }
    }

    private static void LoadDependencies(string bundle, string mod)
    {
        if (!manifests.ContainsKey(mod))
        {
            LoadManifest(mod);
        }

        foreach (string d in manifests[mod].GetAllDependencies(bundle))
        {
            if (!assetbundles.ContainsKey(d)) LoadAssetBundle(d, mod);
        }
    }

    private static void UnloadDependencies(string bundle, string mod, bool keepObjects)
    {
        if (!manifests.ContainsKey(mod))
        {
            LoadManifest(mod);
        }

        foreach (string d in manifests[mod].GetAllDependencies(bundle))
        {
            DisposeAssetBundle(d, mod, keepObjects);
        }
    }

    private static void LoadManifest(string mod)
    {
        string path;
        if (mod == "Area") path = Application.dataPath + "/AssetBundles/AssetBundles";
        else path = $"Mods/Temp/{mod}/AssetBundles/AssetBundles";

        var manifest = AssetBundle.LoadFromFile(path).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if (manifest == null) throw new NullReferenceException("Assetbundlemanifest is not found. {" + path + "}");
        manifests.Add(mod, manifest);
    }
#endif
}