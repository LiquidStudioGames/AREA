using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace wolfingame
{
    public class SteamCreateLobbyTest : MonoBehaviour
    {
        public Game game;

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                game.Steam.CreateLobby();
            }
        }
    }
}
