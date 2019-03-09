using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KbPlayerInput : MonoBehaviour, IPlayerInput
{



    [SerializeField]
    private KeyboardLayout KeyBindings; //Snipped for future keybind change support <Low Prio>


    //Properties for input reading
    public bool JumpingPressed { get; private set; }
    public bool JumpingHeld { get; private set; }
    public bool FirePressed { get; private set; }
    public bool FireHeld { get; private set; }
    public bool Forward { get; private set; }
    public bool Backwards { get; private set; }
    public bool Left { get; private set; }
    public bool Right { get; private set; }



    //Needs working
    public void ReadInput()
    {
        JumpingPressed = Input.GetKeyDown(KeyBindings.Jump);
        JumpingHeld = Input.GetKey(KeyBindings.Jump);
        FirePressed = Input.GetKeyDown(KeyBindings.Fire);
        FireHeld = Input.GetKey(KeyBindings.Fire);
        Forward = Input.GetKey(KeyBindings.Forward);
        Backwards = Input.GetKey(KeyBindings.Backwards);
        Left = Input.GetKey(KeyBindings.StrafeLeft);
        Right = Input.GetKey(KeyBindings.StrafeRight);
        
    }


    // Awake is called when the object is enabled
    void Awake()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
