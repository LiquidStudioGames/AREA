using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModReader : MonoBehaviour
{
    void Start()
    {
        // Change directory on release
        DirectoryInfo d = new DirectoryInfo(@'D:\Users\Daniel\Documents\GitHub\fast-paced-fps\Fast Paced FPS\Assets\Gamemodes');//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles('*.gmc'); //Getting Text files
        foreach (FileInfo file in Files)
        {
            //Debug.Log(file.Name);
            ReadMod(file);
        }
    }

    void ReadMod (FileInfo file)
    {
        string content;

        // Reads the mod file, and put it in the string content
        StreamReader reader = new StreamReader(file.FullName);
        content = reader.ReadToEnd();
        reader.Close();

        //Debug.Log(content);

        // Removes the file ending
        string name = file.Name.Replace('.gmc', '');

        // Runs the method for handling the mods
        ModLoader.Instance.LoadMod(name, 'Gamemode', content);
    }

}
