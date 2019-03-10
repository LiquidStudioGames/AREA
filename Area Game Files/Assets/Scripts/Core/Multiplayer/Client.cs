using UnityEngine;

using Photon.Pun;

public class Client : MonoBehaviour
{

    #region Singleton
    public static Client client;

    void Awake ()
    {
        // Creates a singleton
        client = this;
    }
    #endregion

    // Start Method :: Called on start
    void Start ()
    {
        // Connects to the Photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    // Import Photon 'On' methods, for error checking

    // Updated Method :: Called each frame
    void Update () { }

}
