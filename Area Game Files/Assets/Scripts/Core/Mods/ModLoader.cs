using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Ionic.Zip;
using Newtonsoft.Json;

public class Mod
{
    public string Name { get; set; }
    public List<string> Assemblies { get; set; }
}

public class ModLoader
{
    private const string path = "Mods";
    private const string temppath = "Mods/Temp";

    private Dictionary<string, Mod> mods;

    public IEnumerable<string> Mods => mods.Keys;
    public Mod this [string mod] => mods[mod];

    public void LoadMods()
    {
        mods = new Dictionary<string, Mod>();
        if (Directory.Exists(temppath)) Directory.Delete(temppath, true);
        Directory.CreateDirectory(temppath);

        foreach (string mod in Directory.GetFiles(path))
        {
            LoadMod(mod);
        }
    }

    private void LoadMod(string mod)
    {
        string name = Path.GetFileNameWithoutExtension(mod);

        // Check zip
        if (Path.GetExtension(mod) != "zip") return;
        
        // Unzip file
        using (ZipFile file = new ZipFile(mod))
            file.ExtractAll(temppath + "/" + name);

        // Load zip.json
        Mod config = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(temppath + $"/{name}/{name}.json"));

        // Load assemblies
        foreach (string dll in config.Assemblies)
            Assembly.LoadFile(temppath + "/" + name + "/" + dll);
    }
}
