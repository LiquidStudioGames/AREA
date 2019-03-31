using System.Collections;
using System.Collections.Generic;
using UnityEngine;




///<summary>
/// Class used to aid PlayerMovement in collisions 
///</summary>
[RequireComponent(typeof(PlayerMovementFixed))]
public class PlayerEnviromentChecker : MonoBehaviour
{

    private Transform playerPos;
    private CharacterController charCont;
    private RaycastHit hit;
    private bool isGrounded;
    private bool onSlope; //Might not be used 
    private float playerHeight;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float grOffset = 0.1f;


    //Properties to access from outside
    public bool OnSlope { get => onSlope; }
    public bool IsGrounded { get => isGrounded; }
    public RaycastHit Hit { get => hit; }

    private bool GroundCheck()
    {
        Ray sphereRay = new Ray(transform.position, -transform.up );
        if (Physics.SphereCast(sphereRay, charCont.radius, out hit, playerHeight + grOffset - charCont.radius, layerMask))
        {
            onSlope = true;
            return true;
        }
        onSlope = false;
        return false;

    }

    private void Start()
    {
        charCont = GetComponentInParent<CharacterController>();
        playerPos = GetComponentInParent<Transform>();
        playerHeight = charCont.bounds.extents.y;
    }

    private void FixedUpdate() { isGrounded = GroundCheck(); }

}
