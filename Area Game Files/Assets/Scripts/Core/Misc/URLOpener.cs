using UnityEngine;

public class URLOpener : MonoBehaviour
{
    
    // OpenURL Method :: Opens a given url in the browser
    public void OpenUrl (string URL)
    {

        // Opens the given url
        System.Diagnostics.Process.Start(URL);

    }

}
