using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KbPlayerInput : MonoBehaviour, IPlayerInput
{


    //Properties for input reading
    public bool JumpingPressed { get; private set; }
    public bool JumpingHeld { get; private set; }
    public bool FirePressed { get; private set; }
    public bool FireHeld { get; private set; }





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Needs working
    public void ReadInput()
    {
        throw new NotImplementedException();
    }

}
