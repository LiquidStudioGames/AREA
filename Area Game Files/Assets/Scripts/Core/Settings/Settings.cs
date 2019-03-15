using UnityEngine;

using Newtonsoft.Json;

using System;
using System.IO;

public class Settings
{

    // Creates a string called path, which stores the settings path
    public string Path => Application.persistentDataPath + "/settings.json";

    // Creates a public SettingsData variable with the name Data
    public SettingsData Data;
    
    // Load Method :: Reads the setting file and assign the data to a variable
    public void Load ()
    {

        if (File.Exists(Path))
        {
            // Creates a string called text, which stores the files content
            string Text = File.ReadAllText(Path).ToString();

            try
            {
                // This is to load the settings, it's never a list
                Data = JsonConvert.DeserializeObject<SettingsData>(Text);
            }

            catch (Exception e)
            {
                Debug.LogWarning("Failed loading settings, using default settings: " + e);
                Data = new SettingsData();
            }
        }

        else Data = new SettingsData();

        if (Data.GameStats)
        {
            Game.Instance.UI.Stats.SetActive(true);
        }

        else
        {
            Game.Instance.UI.Stats.SetActive(false);
        }

    }

    // Save Method :: Saves the changed settings to the settings file
    public void Save ()
    {

        // Convert the data to a JSON object
        string Json = JsonConvert.SerializeObject (Data);

        // Write the JSON data to the file
        File.WriteAllText (Path, Json);

        if (Data.GameStats)
        {
            Game.Instance.UI.Stats.SetActive(true);
        }

        else
        {
            Game.Instance.UI.Stats.SetActive(false);
        }

    }

}
