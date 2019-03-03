using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsGun : MonoBehaviour
{
    public AbsBullet GunBullet; //Bullet that will shoot 

    //Stats for any kind of weapon
    public float FireRate;
    public float ReloadTime;
    public int MaxAmmo;
    public int CurrentAmmo;




    // Start is called before the first frame update
    public virtual void Start() { }

    // Update is called once per frame
    public virtual void Update(){ }
}


public abstract class AbsBullet : MonoBehaviour
{




}