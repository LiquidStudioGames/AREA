using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// PlayerReferencer will be the class that will hold all the player information to ease both the transfer of information and its editing
/// </summary>

public class playerReferencer : MonoBehaviour //it's only a monobehaviour because of [Serialize]
{
    private Camera cam;
    private Transform playerLocation;


    public Camera Cam { get => cam;}
    public Transform PlayerLocation { get => playerLocation; set => playerLocation = value; }








    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        playerLocation = GetComponent<Transform>();
    }

}
