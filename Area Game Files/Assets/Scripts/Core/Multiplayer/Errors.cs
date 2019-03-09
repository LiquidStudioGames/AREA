using UnityEngine;
using UnityEngine.UI;

class Errors : MonoBehaviour
{

    // Sets the error messages to the errors
    public string Error_NameSpecify = "Please specify a name for the game!";
    public string Error_ConnectToServer = "Failed to connect to server!";
    public string Error_ConnectToGame = "Failed to connect to the game!";
    public string Error_CreateGame = "Failed to create the game!";
    public string Error_JoinGame = "Failed to join the game!";

    public static Errors Instance;

    // Gets the status text element
    public Text Status;

    void Awake()
    {
        Instance = this;
    }

    // SetErrorStatus Method :: Sets the status text to the error
    public void SetErrorStatus(string Error)
    {
        Status.text = Error;
    }

}