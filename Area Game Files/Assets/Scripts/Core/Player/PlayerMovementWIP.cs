using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementWIP : MonoBehaviour
{

    [SerializeField]
    private LayerMask groundCollision;



    // Movement factors 
    public float gravity = 20.0f;
    public float downGravityMultiplier = 1.2f;    //gravity mult for falls, higher than one = weightier falls
    public float friction = 6;                    //Ground friction
    public float baseSpeed = 7.0f;                // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public bool holdJumpToBhop = false;           // When enabled allows player to just hold jump button to keep on bhopping perfectly.

    // Internals for class control
    private float playerHeight;
    private Vector3 playerVelocity = Vector3.zero;
    private Vector3 playerDireccion = Vector3.zero;
    private float groundSmooth = 0.1f; //Extra distance to smooth out rough terrain
    private CharacterController chController; //will be used for caracter collision
    private IPlayerInput reader;
    private bool wishJump;
    private Vector3 moveDirNorm;
    private Transform parentTransform;
    private float wishSpeed;


    // Awake is called when object is enabled
    void Awake()
    {
        reader = GetComponent<IPlayerInput>();
        chController = GetComponentInParent<CharacterController>();
        playerHeight = chController.bounds.extents.y;
        parentTransform = GetComponentInParent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Debug part of the code */
        Debug.DrawRay(transform.position, -transform.up * (playerHeight + groundSmooth), Color.red);

        // Movement part 
        QueueJump();
        if (IsGrounded())
        {
            GroundMove();
        } else 
        {
            AirMove();
        }







    }

    private void AirMove()
    {
        throw new NotImplementedException();
    }

    private void GroundMove()
    {
        // only apply friction if the player is not queuing a jump
        if (!wishJump)
        {
            ApplyFriction(1f);
        }

        moveDirNorm = DesireDirection();
        wishSpeed = moveDirNorm.magnitude;
        wishSpeed *= baseSpeed;
        Accelerate(DesireDirection(), wishSpeed, runAcceleration);


        //Resets gravity if grounded
        playerVelocity.y = -gravity * Time.deltaTime;

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }

    }

    private void Accelerate(Vector3 vector3, float wishSpeed, float runAcceleration)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Transforms input booleans to a vector3 used for horizontal plane movement
    /// </summary>

    private Vector3 DesireDirection()
    {

        Vector3 result = new Vector3((reader.Right ? 1 : 0) + (reader.Left ? -1 : 0), 0, (reader.Forward ? 1 : 0) + (reader.Backwards ? -1 : 0));
        result = parentTransform.TransformDirection(result);
        result.Normalize();
        return result;

    }

    private void ApplyFriction(float fMultiplier)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Queues Jump 
    /// </summary>
    private void QueueJump()
    {
        if (holdJumpToBhop)
        {
            wishJump = reader.JumpingHeld;
            return;
        }

        if (reader.JumpingPressed && !wishJump)
        {
            wishJump = true;
        }
        else
        {
            wishJump = false;
        }
            
    }


    /// <returns>
    /// Retruns true if anything is under the player's feet
    /// </returns>
    private bool IsGrounded()
    {

        return Physics.Raycast(transform.position, -transform.up, playerHeight+groundSmooth, groundCollision);

    }

}
