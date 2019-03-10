using Photon.Pun;
using UnityEngine;

using Photon.Realtime;

using System.Collections.Generic;

public class Callbacks : MonoBehaviourPunCallbacks
{

    private Errors error;

    void Awake()
    {
        error = Errors.Instance;
    }

    // This gets called whenever we connects to the server
    public override void OnConnectedToMaster ()
    {
        // Prints out to the console
        Debug.Log("Connected to server");
    }

    // This gets called whenever a new game gets added to the list
    public override void OnRoomListUpdate(List<RoomInfo> GameList)
    {
        Debug.Log(GameList);

        // TODO: Add the game (with it's info) to the UI game list
    }

    // This gets called whenever we fails to create a game
    public override void OnCreateRoomFailed (short returnCode, string message)
    {
        // Sets the status to the CreateGame error message
        error.SetStatus(error.Error_CreateGame);
    }

    // This gets called whenever we fails to join a game
    public override void OnJoinRoomFailed (short returnCode, string message)
    {
        // Sets the status to the JoinGame error message
        error.SetStatus(error.Error_JoinGame);
    }

    // This gets called whenever we fails to join a random game
    public override void OnJoinRandomFailed (short returnCode, string message)
    {
        // Sets the status to the JoinGame error message
        error.SetStatus(error.Error_JoinGame);
    }

}
