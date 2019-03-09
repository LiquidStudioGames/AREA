using UnityEngine;

class Errors : MonoBehaviour
{

  string Error_NameSpecify = "Please specify a name for the game!";
  string Error_ConnectToServer = "Failed to connect to server!";
  string Error_ConnectToGame = "Failed to connect to the game!";
  
  public static Errors Instance;
  
  void Awake ()
  {
    Instance = this;
  }

}
