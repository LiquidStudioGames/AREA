using UnityEngine;

class Errors : MonoBehavior
{

  Error_NameSpecify = "Please specify a name for the game!";
  
  public static Errors Instance;
  
  void Awake ()
  {
    Instance = this;
  }

}
