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
        string path = Application.persistentDataPath + "/settings.json";

        // Checks if the settings file doesn't exist
        if (!File.Exists(path))
        {

            // If it doesn't, then create a list of settings data
            List<SettingsData> Data = new List<SettingsData>();

            // Add the settings data to the list
            Data.Add(new SettingsData()
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
            File.WriteAllText(path, Json);

        }

    }

}

// Settings data class
public class SettingsData
{
    public int GraphicsIndex;       // The index of graphics quality
    public bool Music;              // A bool for whether music should be on or not
    public bool Sounds;             // A bool for whether sounds should be on or not
    public float MusicVolume;       // The volume of the music
    public float SoundsVolume;      // The volume of the sounds 
    public bool AutoReload;         // A bool for whether it should auto reload or not
    public bool GameStats;          // A bool for whether it should show the game stats or not
}
