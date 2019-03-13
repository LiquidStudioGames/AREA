using UnityEngine;

using Newtonsoft.Json;

using System.IO;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{

    // Creates an instance variable of the script
    public static Settings Instance;

    // Creates a public SettingsData variable with the name Data
    public SettingsData Data;

    // TODO: Take all the value inputs in here

    // Awake Method :: Gets called at the very start
    void Awake ()
    {

        // Sets the instance variable to this
        Instance = this; 

    }

    // Start Method :: Gets called at the start
    void Start ()
    {

        // Creates a string called path, which stores the settings path
        string Path = Application.persistentDataPath + "/settings.json";

        // Creates a string called text, which stores the files content
        string Text = File.ReadAllText(Path).ToString();

        // This is if the settings file is a list
        try
        {

            // Gets a list of all the elements as a list
            List<SettingsData> Datas = JsonConvert.DeserializeObject<List<SettingsData>> (Text);

            // Then sets the data variable to the first item in the list
            Data = Datas[0];

        }

        // This is if it's a single item file
        catch
        {

            // Sets the data variable to the file contents
            Data = JsonConvert.DeserializeObject<SettingsData> (Text);

        }

    }

    // SaveSettings Method :: Saves the changed settings to the settings file
    public void SaveSettings ()
    {

        // Creates a string called path, which stores the settings path
        string Path = Application.persistentDataPath + "/settings.json";

        // If it doesn't, then create a list of settings data
        List<SettingsData> Data = new List<SettingsData>();

        // Add the settings data to the list
        Data.Add (new SettingsData()
        {

            // Assign all the settings data settings
            // TODO: Get the value inputs and set them here
            GraphicsIndex = 0,
            Music = true,
            Sounds = true,
            MusicVolume = 100.0f,
            SoundsVolume = 100.0f,
            AutoReload = false,
            GameStats = false,

        });

        // Convert the data to a JSON object
        string Json = JsonConvert.SerializeObject (Data.ToArray());

        // Write the JSON data to the file
        File.WriteAllText(Path, Json);

    }

}
