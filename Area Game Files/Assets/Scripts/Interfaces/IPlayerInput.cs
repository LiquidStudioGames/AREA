using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInput
{
    /// <summary>
    /// Interface used to streamline the creation of PlayerInput classes
    /// </summary>


    bool JumpingPressed { get; }
    bool JumpingHeld { get; } //Used for bufferjumping

    bool FirePressed { get; }
    bool FireHeld { get; } //Used when AutoFire is on

    bool Forward { get; }
    bool Backwards { get; }
    bool Left { get; }
    bool Right { get; }
        

    void ReadInput();


}
