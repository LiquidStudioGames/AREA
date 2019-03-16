using UnityEngine;
using UnityEngine.UI;

public class AccountUI : MonoBehaviour
{
    public Text Username;
    
    private MenuStatus Status;
    private SteamClient Steam => Game.Instance.Steam;
    
    void Start ()
    {
        Username.text = Steam.Player.Name + "@" + Steam.Player.ID.ToString().Substring(0, 7);
    }
}
