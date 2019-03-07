using UnityEngine;

public class Callbacks : MonoBehaviourPunCallbacks
{
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
    }
    
}
