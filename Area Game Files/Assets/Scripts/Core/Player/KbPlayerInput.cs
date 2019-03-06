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

    private Input Reader = new Input();


    // Awake is called when the object is enabled
    private void awake()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }


    //Needs working
    public void ReadInput(Input reader)
    {
        throw new NotImplementedException();
    }

}
