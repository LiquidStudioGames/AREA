using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementWIP : MonoBehaviour
{

    [SerializeField]
    private LayerMask GroundCollision;
    [SerializeField]
    private float GroundSmooth = 0.1f; //Extra distance to smooth out rough terrain



    private IPlayerInput reader;


    //Internals for class control
    private float playerHeight;


    /// <returns>
    /// Retruns true if anything is under the player's feet
    /// </returns>
    private bool IsGrounded()
    {

        return Physics.Raycast(transform.position, -transform.up, playerHeight+GroundSmooth, GroundCollision);

    }





    // Awake is called when object is enabled
    void Awake()
    {
        reader = GetComponent<IPlayerInput>();

        playerHeight = GetComponent<Collider>().bounds.extents.y;


    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(transform.position, -transform.up * (playerHeight + GroundSmooth), Color.yellow);

    }
}
