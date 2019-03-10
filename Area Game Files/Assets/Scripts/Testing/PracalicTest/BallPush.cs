using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPush : MonoBehaviour
{
    public LayerMask layer;
    int layerMask;// = LayerMask.GetMask("Water");

    void Start()
    {
        layerMask = LayerMask.GetMask("Water");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            //{
            //    Debug.Log("Raycast");
            //}
            int layer_mask = LayerMask.GetMask("Default");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                Debug.Log("Trafiony  " + objectHit.name);

                Rigidbody rig = objectHit.GetComponent<Rigidbody>();
                if(rig != null)
                    rig.AddForce(new Vector3(Random.Range(-100, 100f), 0, Random.Range(-100, 100f)));
                // Do something with the object that was hit by the raycast.
            }
        }
    }
}
