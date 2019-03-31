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
    private int layerMask;
    [SerializeField]
    private float grOffset = 0.1f;


    //Properties to access from outside
    public bool OnSlope { get => onSlope; }
    public bool IsGrounded { get => isGrounded; }
    public RaycastHit Hit { get => hit; }

    private bool GroundCheck()
    {
        bool v = Physics.Raycast(playerPos.position, -playerPos.up, out hit, playerHeight+grOffset, layerMask);

        if (v)
        {
            if (hit.normal != transform.up)
                onSlope = true;

            return true;
        }
        return false;
      
    }

    private void Start()
    {
        charCont = GetComponentInParent<CharacterController>();
        playerPos = GetComponentInParent<Transform>();
        playerHeight = charCont.bounds.extents.y;
    }
    private void FixedUpdate() { GroundCheck(); }

}
