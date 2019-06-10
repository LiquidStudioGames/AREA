using System.Collections.Generic;
using System.IO;

public class Mod
{
    public string Name { get; set; }
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

        foreach (string mod in Directory.GetFiles(path))
        {
            LoadMod(mod);
        }
    }

    private void LoadMod(string mod)
    {
        // Check zip
        // Unzip file
        // Load zip.json

        // Following steps should happen when you actually use the mod, 
        // Could be loaded at start aswell since we can't unload assemblies
        // Load assemblies
        // Load assetbundles
    }
}
