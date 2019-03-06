using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Host : MonoBehaviour
{

    private Client client;

    private byte GameSize = 10;
    private string GameName;

    public Text Error;

    // Start Method :: Called on start
    void Start ()
    {
        // Set the client to a Client instance
        client = Client.client;
    }

    // Set game name method
    public void SetName (string Name)
    {
        // Sets the name variable to the given text
        GameName = Name;
    }

    // Create game method
    public void CreateGame ()
    {

        // Checks if the name for the game is set and is not nothing
        if (GameName != null && GameName != "")
        {
            // If it is, then proceed to create the game

            // Sets the game settings :: Visible, open and sets max players to the variable
            RoomOptions GameSettings = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = GameSize
            };

            // Creates the photon room with the game settings
            PhotonNetwork.CreateRoom(GameName, GameSettings);
        }

        // If it's not, then set the error text
        else
        {
            Error.text = "Please specify a name for the game!";
        }

    }

}
