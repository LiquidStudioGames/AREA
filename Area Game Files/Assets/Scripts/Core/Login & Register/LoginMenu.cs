using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DatabaseControl;

public class LoginMenu : MonoBehaviour
{

    // Login and register section
    public GameObject LoginSection;
    public GameObject RegisterSection;

    // All the input fields
    public InputField LoginUsername;
    public InputField LoginPassword;
    public InputField RegisterUsername;
    public InputField RegisterPassword;
    public InputField RegisterConfirmPassword;

    // Error texts
    public Text LoginError;
    public Text RegisterError;

    // Error messages
    string ErrorUsernameLength = "Your username should be at least 5 chars, and max 15 chars";
    string ErrorPasswordLength = "Your password should be at least 8 chars, and max 16 chars";
    string ErrorUsernameExists = "Username not found";
    string ErrorUsernameTaken = "The username is already in use";
    string ErrorPasswordIncorrect = "The password does not match with the username";
    string ErrorPasswordMatch = "The passwords does not match";
    string ErrorUnknown = "Something went wrong. Try again";
    string ErrorEmpty = "Please enter a username and password";

    // Awake Method :: Called at the very start
    void Awake ()
    {
        ResetUI();
    }

    // ResetUI Method :: Resets the input fields
    void ResetUI ()
    {

        // Empties all the fields
        LoginUsername.text = "";
        LoginPassword.text = "";
        RegisterUsername.text = "";
        RegisterPassword.text = "";
        RegisterConfirmPassword.text = "";

        // Calls the ResetErrors method, to empty the errors
        ResetErrors();

    }

    // ResetErrors Method :: Resets the errors
    void ResetErrors ()
    {
        
        // Sets the error texts to nothing
        LoginError.text = "";
        RegisterError.text = "";

    }

    // ButtonLogin Method :: Gets called when the login button is pressed
    public void ButtonLogin ()
    {

        // Sets the username and password to the given text
        string Username = LoginUsername.text;
        string Password = LoginPassword.text;

        // Checks if the username length is at least 5, and max 15
        if (Username.Length >= 5 &&  Username.Length <= 15)
        {
            // Checks if the username length is at least 8
            if (Password.Length >= 8 && Password.Length <= 16)
            {
                // If it is, login the user
                StartCoroutine(LoginUser (Username, Password));
            }

            // If it's not
            else
            {
                // Then set the error text to the error
                LoginError.text = ErrorPasswordLength;
            }
        }

        // Checks if the length of both fields is 0 (They're empty)
        else if (Username.Length == 0 && Password.Length == 0)
        {
            // If it is, then set the error text to the error
            LoginError.text = ErrorEmpty;
        } 

        // If the username is shorter than 5 chars, and longer than 15
        else
        {
            // Then set the error text to the error
            LoginError.text = ErrorUsernameLength;
        }

    }

    // ButtonRegister Method :: Gets called when the register button is pressed
    public void ButtonRegister ()
    {
        // Sets the username and password strings to the given text
        string Username = RegisterUsername.text;
        string Password = RegisterPassword.text;
        string ConfirmPassword = RegisterConfirmPassword.text;

        // Checks if the username is at least 5 chars, and max 15
        if (Username.Length >= 5 && Username.Length <= 15)
        {
            // Checks if the username is at least 8 chars, and max 16
            if (Password.Length >= 8 && Password.Length <= 16)
            {
                // Checks if the two given passwords match
                if (Password == ConfirmPassword)
                {
                    // If they do, then register the user and give it data with it
                    StartCoroutine(RegisterUser (Username, Password, "[KILLS]0/[DEATHS]0/[GAMES]0/[WON]0/[LEVEL]0/[RANK]0"));
                }

                // If they doesn't match
                else
                {
                    // Then set the status text to the error
                    RegisterError.text = ErrorPasswordMatch;
                }
            }

            // If the password is not long enough, or too long
            else
            {
                // Then set the status text to the error
                RegisterError.text = ErrorPasswordLength;
            }
        }

        // If the username is not long enough, or too long
        else
        {
            // Then set the status text to the error
            RegisterError.text = ErrorUsernameLength;
        }
    }

    // SwitchSection Method :: Switch to the other section
    public void SwitchSection ()
    {

        // Calls the ResetUI method to empty the fields
        ResetUI();

        // Switches the sections, by activating the deactivated, and deactivate the activated
        LoginSection.gameObject.SetActive(!LoginSection.activeInHierarchy);
        RegisterSection.gameObject.SetActive(!RegisterSection.activeInHierarchy);

    }

    // LoginUser Method :: Logs in the user
    IEnumerator LoginUser (string Username, string Password)
    {

        // Calls the Login coroutine, and sets the variable e to the result
        IEnumerator e = DCF.Login (Username, Password);

        while (e.MoveNext())
        {
            yield return e.Current;
        }

        // Sets the string result, to the current e element
        string Result = e.Current as string;

        // Checks if it's success
        if (Result == "Success")
        {
            // If it is, then reset the UI, and call the Login method
            ResetUI();
            AccountManager.Instance.Login (Username, Password);
        }

        // If it's not
        else
        {
            // Acticvates the login section
            LoginSection.gameObject.SetActive(true);

            // Checks if the result is a user error (Username is taken)
            if (Result == "UserError")
            {
                // If it is, then set the error text to the error
                LoginError.text = ErrorUsernameExists;
            }

            // If it's not
            else
            {
                // Then check if it's a password error (Username and password does not match)
                if (Result == "PassError")
                {
                    // Then sets the error text to the error
                    LoginError.text = ErrorPasswordIncorrect;
                }

                // If it's not a password error either, then it's an unkown error
                else
                {
                    // Sets the error text to the error
                    LoginError.text = ErrorUnknown;
                }
            }
        }

    }

    // RegisterUser Method :: Registers the user
    IEnumerator RegisterUser (string Username, string Password, string Data)
    {

        // Calls the RegisterUser coroutine and sets the variable e to the result
        IEnumerator e = DCF.RegisterUser (Username, Password, Data);

        while (e.MoveNext())
        {
            yield return e.Current;
        }

        // Sets the result string to the current e element
        string Result = e.Current as string;

        // Checks if the result is success
        if (Result == "Success")
        {
            // If it is, then call the ResetUI method, and login the user
            ResetUI();
            AccountManager.Instance.Login (Username, Password);
        }

        // If it's not a success
        else
        {
            // Then set the register section to active
            RegisterSection.gameObject.SetActive(true);

            // Then check if the result is an user error (username taken)
            if (Result == "UserError")
            {
                // Then set the error text to the error
                RegisterError.text = ErrorUsernameTaken;
            }

            // If it's not a user error, then it's an unkown error
            else
            {
                // Sets the error text to the error
                LoginError.text = ErrorUnknown;
            }
        }

    }

}
