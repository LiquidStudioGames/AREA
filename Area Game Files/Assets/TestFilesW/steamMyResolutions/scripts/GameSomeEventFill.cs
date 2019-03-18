using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace wolfingame.steamTest
{
    public class GameSomeEventFill : MonoBehaviour
    {


        static GameSomeEventFill instance = null;
        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
                Destroy(this.gameObject);

            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Game.Instance.Steam.OnLobbyEvent += LobbyEventDecide;
        }


        void LobbyEventDecide(LobbyEvent ev)
        {
            if (ev == LobbyEvent.Created)
            {

            }
            if (ev == LobbyEvent.Joined)
            {


            }
            if (ev == LobbyEvent.Left)
            {

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
