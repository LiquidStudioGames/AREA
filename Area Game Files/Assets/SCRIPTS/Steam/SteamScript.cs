using UnityEngine;
using System.Collections;
using Steamworks;

namespace pracalic
{
    public class SteamScript : MonoBehaviour
    {
        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

        void Start()
        {
            if (SteamManager.Initialized)
            {
                string name = SteamFriends.GetPersonaName();
                // m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
                Debug.Log(name);
            }
        }



        private void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
                m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            }
        }

        private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
        {
            //this can be used for pause game 
            if (pCallback.m_bActive != 0)
            {
                Debug.Log("Steam Overlay has been activated");
            }
            else
            {
                Debug.Log("Steam Overlay has been closed");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
                m_NumberOfCurrentPlayers.Set(handle);
                Debug.Log("Called GetNumberOfCurrentPlayers()");
            }
        }

        private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
        {
            if (pCallback.m_bSuccess != 1 || bIOFailure)
            {
                Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
            }
            else
            {
                Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers);
            }
        }
    }
}