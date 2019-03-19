using UnityEngine;
using UnityEngine.UI;

using Steamworks;

public class SystemInfo : MonoBehaviour
{

    // Takes in the info window
    public GameObject InfoWindow;

    // Creates the text variables
    private Text FPS_Text;
    private Text Ping_Text;
    private Text CStatus_Text;
    private Text CServer_Text;

    // Creates the info variables
    private int FPS;
    private int Ping;
    private string ConnectionStatus;
    private string ConnectionServer;

    // Start Method :: Called on start
    void Start()
    {

        // Gets all the child elements from the info window and assigns them to the text variables
        FPS_Text = InfoWindow.transform.Find("FPS").gameObject.GetComponent<Text>();
        Ping_Text = InfoWindow.transform.Find("Ping").gameObject.GetComponent<Text>();
        CStatus_Text = InfoWindow.transform.Find("CStatus").gameObject.GetComponent<Text>();
        CServer_Text = InfoWindow.transform.Find("CServer").gameObject.GetComponent<Text>();

    }

    // Start Method :: Called each frame
    void Update()
    {

        // Checks if the info window is active
        if (InfoWindow.activeInHierarchy)
        {
            // If it is, then run the SetInfo method to update the stats
            SetInfo();

            // Then assign the text variables's text element to the info variables
            FPS_Text.text = FPS.ToString();
            Ping_Text.text = Ping.ToString();
            CStatus_Text.text = ConnectionStatus;
            CServer_Text.text = ConnectionServer;
        }

    }

    // SetInfo Method :: Assigns the correct info to the info variables
    private void SetInfo ()
    {

        // Assigns the FPS :: Rounds from float to int
        // This here is not completely correct, but it's quite close
        FPS = Mathf.RoundToInt(1f / Time.deltaTime);

        // Assigns the ping
        Ping = 0;
        
        // Checks if we're connected to the server
        if (SteamAPI.Init())
        {

            // If we are, then assign the connection status and server
            ConnectionStatus = "Connected";
            // TODO: Change this to use Steam instead
            //ConnectionServer = PhotonNetwork.Server.ToString();
            ConnectionServer = "Probably EU";

        }

        else
        {

            CStatus_Text.color = Color.red;
            CServer_Text.color = Color.red;

            // If we're not, then assign them to this
            ConnectionStatus = "Not connected";
            ConnectionServer = "None";

        }

    }

    // ToggleInfo Method :: Toggles the info window
    public void ToggleInfo ()
    {

        InfoWindow.SetActive(!InfoWindow.activeInHierarchy);

    }

}
