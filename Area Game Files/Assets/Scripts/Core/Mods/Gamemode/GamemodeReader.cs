using UnityEngine;

using Newtonsoft.Json;

using System;
using System.IO;

public class GamemodeReader : MonoBehaviour
{

    public GamemodeData Data;

    private Errors error;

    void Awake ()
    {
        error = Errors.Instance;
    }

    public void Read (string Path)
    {

        if (File.Exists (Path))
        {

            // Creates a string called text, which stores the files content
            string Text = File.ReadAllText (Path).ToString ();

            try
            {
                Data = JsonConvert.DeserializeObject<GamemodeData> (Text);
            }

            catch (Exception err)
            {
                Debug.Log(err);
                error.SetStatus("Failed loading gamemode, using default gamemode: TDM");
            }

        }

    }

}
