using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class Join : MonoBehaviour
{

    private Client client;
    private Errors error;

    // Start Method :: Called on start
    void Start ()
    {
        client = Client.client;
        error = Errors.Instance;
    }

    // JoinGame Method :: Joins a game
    public void JoinGame (Text GameName)
    {

        // Sets the status text to the message
        error.SetStatus ("Connecting to game: " + GameName.text + "...");

        // Try and join the game
        PhotonNetwork.JoinRoom(GameName.text);

    }

    // JoinRandomGame Method :: Joins a random game
    public void JoinRandomGame ()
    {

        // Sets the status text to the message
        error.SetStatus ("Connecting to a random game...");

        // Try and join a random open game/lobby
        PhotonNetwork.JoinRandomRoom();

    }

}
