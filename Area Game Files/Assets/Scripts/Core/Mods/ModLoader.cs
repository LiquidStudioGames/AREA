using System.Reflection;
using System.Collections.Generic;
using System.IO;

// 2 Ways
// 1. Always have an assetbundle which main asset is a prefab that holds all mod information (which dll files and assetbundles to load)
// 2. Always have a dll with a class that extends from IMod which holds all mod information (which dll files and assetbundles to load)

public class Mod
{
    public string Name { get; set; }
    public Assembly Assembly { get; set; }
}

public class ModLoader
{
    private const string path = "Mods";

    private Dictionary<string, Mod> mods;

    public IEnumerable<string> Mods => mods.Keys;
    public Mod this [string mod] => mods[mod];

    public void LoadMods()
    {
        mods = new Dictionary<string, Mod>();

        foreach (string dir in Directory.GetDirectories(path))
        {
            LoadMod(dir);
        }
    }

    private void LoadMod(string directory)
    {
        Mod mod = new Mod { Name = Path.GetDirectoryName(directory) };

        foreach (string file in Directory.GetFiles(directory))
        {
            switch (Path.GetExtension(file))
            {
                case "assetbundle":
                    break;

                case "dll":
                    mod.Assembly = Assembly.LoadFile(file);
                    break;
            }
        }

        mods.Add(mod.Name, mod);
    }
}
