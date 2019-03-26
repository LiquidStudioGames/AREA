using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject cam;
    private Transform playerObj;
    private NetworkTag networkTag;
 
    void Start()
    {
        playerObj = GetComponentInParent<Transform>();
       // networkTag = GetComponent<NetworkTag>();

      /*  if (!networkTag.IsMine)
        {
            Destroy(cam);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
