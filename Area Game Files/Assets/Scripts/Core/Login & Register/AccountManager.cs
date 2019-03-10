using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviour
{

    // Makes a instance of this script
    public static AccountManager Instance;

    // Sets the error variable
    private MenuStatus status;

    // Awake Method :: Called at the very start
    void Awake()
    {

        // Makes the error variable to an instance of the MenuStatus script
        status = MenuStatus.Instance;

        // Checks if this scripts instance exists already
        if (Instance != null)
        {
            // If so, destroy it and return
            Destroy(gameObject);
            return;
        }

        // If not, then set the instance to this, and set it to dont destroy
        Instance = this;
        DontDestroyOnLoad(this);

    }

    // Username and password
    public static string UserUsername { get; protected set; }
    private static string UserPassword = "";

    // Bool for checking if the user is logged in
    public static bool LoggedIn { get; protected set; }

    // Scene indexes
    public int MenuSceneIndex;
    public int LoginSceneIndex;

    // Creates a delegate for receiving data. I can't explain good in english lmao
    public delegate void OnDataReceivedCallback (string Data);

    // Login Method :: Sets up variables we can use later
    public void Login (string Username, string Password)
    {
        // Sets the username and password to the given strings
        UserUsername = Username;
        UserPassword = Password;

        // Sets the logged in bool to true
        LoggedIn = true;

        // Loads the menu scene
        SceneManager.LoadScene(MenuSceneIndex);
    }

    // Logout Method :: Sets up variables we can use later
    public void Logout ()
    {

        // Sets the username and password to nothing
        UserUsername = "";
        UserPassword = "";

        // Sets the logged in bool to false
        LoggedIn = false;

        // Loads the login scene, so the user can login again
        SceneManager.LoadScene(LoginSceneIndex);

    }

    // SendData Method :: Sends data to the server
    public void SendData (string Data)
    {

        if (LoggedIn)
        {
            StartCoroutine(SendDataRequest(UserUsername, UserPassword, Data));
        }

    }

    // GetData Method :: Gets data from the server
    public void GetData (OnDataReceivedCallback OnDataReceived)
    {

        if (LoggedIn)
        {
            StartCoroutine(GetDataRequest(UserUsername, UserPassword, OnDataReceived));
        }

    }

    // SendDataRequest Method :: Requests to send data to the server
    IEnumerator SendDataRequest (string Username, string Password, string Data)
    {

        // Calls the SetUserData coroutine to send data, and sets the set variable to the result
        IEnumerator set = DatabaseControl.DCF.SetUserData (Username, Password, Data);

        while (set.MoveNext())
        {
            yield return set.Current;
        }

        // Sets the WWW variable, result, to the current set element
        WWW Result = set.Current as WWW;

        // Finds errors
        if (Result.text == "ContainsUnsupportedSymbol")
        {
            // Sets the status text to the error
            status.SetStatus("Couldn't upload data to server");
        }

        if (Result.text == "Error")
        {
            // Sets the status text to the error
            status.SetStatus("Couldn't upload to server, because it contains Unsupported Symbol '-'");
        }

    }

    // GetDataRequest Method :: Requests to send data to the server
    IEnumerator GetDataRequest (string Username, string Password, OnDataReceivedCallback OnDataReceived)
    {

        // Sets the data string to error for now
        string Data = "ERROR";

        // Calls the GetUserData coroutine to get data, and sets the get variable to the result
        IEnumerator get = DatabaseControl.DCF.GetUserData (Username, Password);

        while (get.MoveNext())
        {
            yield return get.Current;
        }

        // Sets the WWW variable, result, to the current get element
        WWW Result = get.Current as WWW;

        // Finds errors
        if (Result.text == "Error")
        {
            // Sets the status text to the error
            status.SetStatus("Couldn't upload data to server");
        }

        else
        {
            if (Result.text == "ContainsUnsupportedSymbol")
            {
                // Sets the status text to the error
                status.SetStatus("Couldn't receive from server, because it contains Unsupported Symbol '-'");
            }

            else
            {
                // Sets the received data to a string
                string DataRecieved = Result.text;
                // Sets the data variable to the data
                Data = DataRecieved;
            }
        }

        // Checks if it's not null
        if (OnDataReceived != null)
        {
            // Then invoke it with the data
            OnDataReceived.Invoke(Data);
        }
            
    }

}
