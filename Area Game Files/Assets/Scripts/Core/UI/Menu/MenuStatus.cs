using UnityEngine;
using UnityEngine.UI;

public class MenuStatus : MonoBehaviour
{

    // Creates an instance of this script
    public static MenuStatus Instance;

    // Awake Method :: Gets called in the very start
    void Awake ()
    {

        // Sets the instance to this script
        Instance = this;

    }

    // Takes in the status text
    public Text Status;

    // Start Method :: Gets called in the start
    void Start ()
    {

        // Sets the status text to the message
        Status.text = "Logged in. Connected. Ready!";

    }

    // SetStatus Method :: Sets the status text
    public void SetStatus (string Text)
    {

        // Sets the status text to the given text
        Status.text = Text;

    }

}
