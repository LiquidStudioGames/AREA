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
    private bool isGrounded;
    private bool onSlope;
    



    public bool OnSlope { get => onSlope; }
    public bool IsGrounded { get => isGrounded; }

    private bool GroundCheck()
    {
        onSlope = true;
        return true;
    }

    private void Start() { playerPos = GetComponentInParent<Transform>(); }
    private void FixedUpdate() { GroundCheck(); }

}
