using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModLoader : MonoBehaviour
{

    #region Instance
    public static ModLoader Instance;

    void Awake()
    {
        Instance = this;
    }
    #endregion

    public void LoadMod (string name, string type, string content)
    {
        Debug.Log(name);
        Debug.Log(type);
        Debug.Log(content);

        //Mod name = new Mod(content)

        //GMPresets.Add(TDM);

        if (type == 'Gamemode')
        {
          // The mod type is a gamemode

          if (content[0].ToLower() == 'gamemode:')
          {
            // Proceed to check the file

            cnt = content.Replace('-', '')
          }
        }
    }

}
