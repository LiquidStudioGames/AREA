using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface used to streamline the creation of PlayerInput classes
/// </summary>
public interface IPlayerInput
{



    //Gun values
    bool Reload { get; }
    bool FirePressed { get; set; }
    bool FireHeld { get; } //Used when AutoFire is on


    //Movement values
    bool Forward { get; }
    bool Backwards { get; }
    bool Left { get; }
    bool Right { get; }
    bool JumpingPressed { get; set; }
    bool JumpingHeld { get; } //Used for bufferjumping

    void ReadInput();


}
