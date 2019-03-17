using UnityEngine;

using Newtonsoft.Json;

using System.Collections.Generic;

public class CreateFile : MonoBehaviour
{

    public void MakeFile (string FileName)
    {

        List<Data> data = new List<Data> ();

        data.Add(new Data()
        {
            Type = "TDM",
            Time = 10,
            Rounds = 3,
            Teams = 2,
            TeamSize = 5,
            AmmoSize = 100,
            Health = 100,
            Damage = 100,
            Zonya = true,
            Eyal = true,
            Eqa = true
        });

        string json = JsonConvert.SerializeObject (data.ToArray());

        string path = Application.persistentDataPath + "/Gamemodes/" + FileName + ".json";

        System.IO.File.WriteAllText(path, json);

    }

}

public class Data
{

    public string Type { get; set; }        // The type of gamemode
    public int Time { get; set; }           // The time of each round in minutes
    public int Rounds { get; set; }         // The amount of rounds before game is over
    public int Teams { get; set; }          // The amount of teams
    public int TeamSize { get; set; }       // The amount of players on each team
    public int AmmoSize { get; set; }       // The difference of default ammo (100%)
    public int Health { get; set; }         // The difference of default health (100%) 
    public int Damage { get; set; }         // The difference of default damage (100%)
    public bool Zonya { get; set; }         // If Zonya should be pickable or not
    public bool Eyal { get; set; }          // If Eyal should be pickable or not
    public bool Eqa { get; set; }           // If Eqa should be pickable or not

}