using UnityEngine;

using Photon.Pun;

public class Join : MonoBehaviour
{

    private Client client;

    // Start Method :: Called on start
    void Start ()
    {
        client = Client.client;
    }

    // Join game method
    public void JoinGame () { }

    // Join random game method
    public void JoinRandomGame ()
    {
        // Try and join a random open game/lobby
        PhotonNetwork.JoinRandomRoom();
    }

}
