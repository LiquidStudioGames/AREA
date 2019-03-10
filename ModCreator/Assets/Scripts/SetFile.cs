using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class SetFile : MonoBehaviour
{

    private Data data;

    public void GetData (string FileName)
    {

        string path = Application.persistentDataPath + "/Gamemodes/" + FileName + ".json";

        using (StreamReader file = File.OpenText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            Data _data = (Data)serializer.Deserialize(file, typeof(Data));
            Debug.Log(_data);
        }

    }

    public void SetData (string Parent, string Value)
    {

        Debug.Log(data);

    }

}