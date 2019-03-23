using UnityEngine;

using Newtonsoft.Json;

using System;
using System.IO;

public class GamemodeReader : MonoBehaviour
{

    public GamemodeData Data;

    public void Read (string Path)
    {

        if (File.Exists (Path))
        {
            // Creates a string called text, which stores the files content
            string Text = File.ReadAllText (Path).ToString ();

            try
            {
                // This is to load the settings, it's never a list
                Data = JsonConvert.DeserializeObject<GamemodeData> (Text);
            }

            catch (Exception e)
            {
                Debug.LogWarning("Failed loading gamemode, using default gamemode: " + e);
                Data = new GamemodeData ();
            }
        }

        else Data = new GamemodeData ();

    }

}
