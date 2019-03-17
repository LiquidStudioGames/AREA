using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Transform playerObj; 

    //Gets component 
    void Awake()
    {
        playerObj = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
