using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace wolfingame.steamTest
{
    public class OptionSelector : MonoBehaviour
    {
       // public MonoBehaviour[] monoScript;

        // Start is called before the first frame update
        void Start()
        {
            int option = GameSomeEventFill.Instance.Option;

            if (option > 0)
            {
                if (option == 1)
                {
                    //host client
                    MakeStufForHostClient();
                }
                if (option == 2)
                {
                    //client
                    MakeStufForClientOnly();

                }

            }
            else
            {
                Destroy(GameSomeEventFill.Instance.gameObject);
                SceneManager.LoadScene(0);
            }
        }


        void MakeStufForClientOnly()
        {


        }


        void MakeStufForHostClient()
        {

        }
    }
}
