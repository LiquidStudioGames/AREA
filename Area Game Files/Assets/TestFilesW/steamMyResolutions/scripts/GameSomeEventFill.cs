using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace wolfingame.steamTest
{
    public class GameSomeEventFill : MonoBehaviour
    {
        int option = 0;

        static GameSomeEventFill instance = null;

        public static GameSomeEventFill Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            StartCoroutine(WaitAndFill());
        }

        public int Option
        {
            get
            {
                return option;
            }

        }

        IEnumerator WaitAndFill()
        {
            yield return new WaitForSeconds(0.1f);
            StartInit();
        }
        // Start is called before the first frame update
        void StartInit()
        {
            if (instance != null)
                Destroy(this.gameObject);

            instance = this;
            //DontDestroyOnLoad(this.gameObject);
            Game.Instance.Steam.OnLobbyEvent += LobbyEventDecide;
            Game.Instance.Steam.OnLobbyUpdated += LobbyUpdate;
        }


        void LobbyUpdate(SteamPlayer pl, PlayerStateChange change)
        {
            if (change == PlayerStateChange.Disconnected)
            {
                Debug.Log("Player with id " + pl.ID + " called like a " + pl.Name + " disconnect");
            }

            if (change == PlayerStateChange.Banned)
            {
                Debug.Log("Player with id " + pl.ID + " called like a " + pl.Name + " banned");
            }

            if (change == PlayerStateChange.Joined)
            {
                Debug.Log("Player with id " + pl.ID + " called like a " + pl.Name + " join");
            }

            if (change == PlayerStateChange.Kicked)
            {
                Debug.Log("Player with id " + pl.ID + " called like a " + pl.Name + " kicked");
            }

            if (change == PlayerStateChange.Left)
            {
                Debug.Log("Player with id " + pl.ID + " called like a " + pl.Name + " left game");

            }
        }


        void LobbyEventDecide(LobbyEvent ev)
        {
            if (ev == LobbyEvent.Created)
            {
                Debug.Log("Decide create");
                option = 1;
            }
            if (ev == LobbyEvent.Joined)
            {
                Debug.Log("Decide join");
                option = 2;

            }
            if (ev == LobbyEvent.Left)
            {
                Debug.Log("Decide left");
            }

            if (ev == LobbyEvent.Updated)
            {
                Debug.Log("Decid update");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
