using UnityEngine;

using Newtonsoft.Json;

using System.IO;
using System.Collections.Generic;

public class MenuLoader : MonoBehaviour
{

    // Start Method :: Gets called on start
    void Start ()
    {

        // Sets the settings path
        string Path = Application.persistentDataPath + "/settings.json";

        // Checks if the settings file doesn't exist
        if (!File.Exists(Path))
        {

            // If it doesn't, then create a list of settings data
            List<SettingsData> Data = new List<SettingsData> ();

            // Add the settings data to the list
            Data.Add (new SettingsData()
            {

                // Assign all the settings data settings
                GraphicsIndex = 0,
                Music = true,
                Sounds = true,
                MusicVolume = 100.0f,
                SoundsVolume = 100.0f,
                AutoReload = false,
                GameStats = false,

            });

            // Convert the data to a JSON object
            string Json = JsonConvert.SerializeObject(Data.ToArray());

            // Write the JSON data to the file
            File.WriteAllText(Path, Json);

        }

        // Calls the GetSettings Method to read the file
        Settings.Instance.GetSettings();

    }

}
