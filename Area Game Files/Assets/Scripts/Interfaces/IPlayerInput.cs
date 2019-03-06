using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInput
{
    /// <summary>
    /// Interface used to streamline the creation of PlayerInput classes WIP
    /// </summary>


    bool JumpingPressed { get; }
    bool JumpingHeld { get; } //Used for bufferjumping

    bool FirePressed { get; }
    bool FireHeld { get; } //Used when AutoFire is on


        

    void ReadInput(Input reader);


}
