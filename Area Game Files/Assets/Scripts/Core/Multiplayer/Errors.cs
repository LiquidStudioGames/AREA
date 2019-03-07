using UnityEngine;

class Errors : MonoBehavior
{

  Error_NameSpecify = "Please specify a name for the game!";
  Error_ConnectToServer = "Failed to connect to server!";
  Error_ConnectToGame = "Failed to connect to the game!";
  
  public static Errors Instance;
  
  void Awake ()
  {
    Instance = this;
  }

}
