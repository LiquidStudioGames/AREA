using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public Camera cam;
    public float fireRange = 100f;
    public float hitDamage = 15f;
    public Rigidbody rb;
    public float knockback = -5f;

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shooting();
        }
    }

    void Shooting ()
    {
        RaycastHit targetHit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out targetHit, fireRange))
        {
            Debug.Log("You hit: " + targetHit.transform.name);
            Debug.Log(transform.name + " shot " + targetHit.transform.name);

            if (targetHit.transform.tag == "Environment")
            {
                transform.Translate(1f * Vector3.left * Time.deltaTime);
            }

            Damage target = targetHit.transform.GetComponent<Damage>();
            if (target != null)
            {
                target.DoDamage(transform.name, hitDamage);
            }
        }

    }

}