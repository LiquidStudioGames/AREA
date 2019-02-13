using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public float health = 100f;

    public void DoDamage (string killer, float dmg)
    {
        Debug.Log(dmg);
        health -= 15f;
        if (health <= 0f)
        {
            Die(killer);
        }
    }

    void Die (string killer)
    {
        Debug.Log(killer + " killed " + transform.name + " with a gun");
        Destroy(gameObject);
    }

}
