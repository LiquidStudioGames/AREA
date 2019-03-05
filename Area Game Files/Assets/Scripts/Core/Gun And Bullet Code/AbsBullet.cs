using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsBullet : MonoBehaviour
{
    [SerializeField]
    protected float muzzleVel; //velocity in m/s (1 unit = 1 meter)
    [SerializeField]
    protected int damage;

    protected Vector3 angle;
}
