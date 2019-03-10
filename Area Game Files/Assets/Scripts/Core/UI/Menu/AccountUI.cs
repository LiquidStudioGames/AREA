using UnityEngine;
using UnityEngine.UI;

public class AccountUI : MonoBehaviour
{

    // Takes in the username text
    public Text Username;

    // Crates a MenuStatus variable, called status
    private MenuStatus Status;

    void Awake ()
    {

        // Sets the status variable to an instance of menustatus
        Status = MenuStatus.Instance;
    }

    // Start Method :: Gets called in the start
    void Start ()
    {

        // Checks if the user is logged in
        if (AccountManager.LoggedIn)
        {

            // If so, set the username text to the logged in username
            Username.text = AccountManager.UserUsername;

        }

        // If not
        else
        {

            // Then set the status text to the message
            Status.SetStatus("You're not logged in");

            // Why wouldn't you be logged in lmao?

        }

    }

    // ButtonLogout Method :: Gets called when clicking on the logout menu button
    public void ButtonLogout ()
    {

        // Checks if the user is logged in
        if (AccountManager.LoggedIn)
        {

            // If you are, then call the logout method
            AccountManager.Instance.Logout();

        }

        // If not
        else
        {

            // Then set the status text to the message
            Status.SetStatus("You're not logged in");

            // Why wouldn't you be logged in lmao?

        }

    }

}
