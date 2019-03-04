using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbsGun : MonoBehaviour
{
    public AbsBullet GunBullet; //Bullet that will shoot 

    //Stats for any kind of weapon
    public abstract float FireRate { get; set; }  //Unit in rounds per minute
    public abstract float ReloadTime { get; set; } //Unit in seconds
    public abstract int MaxAmmo { get; set; }
    public abstract int CurrentAmmo { get; set; }
    

    public bool isAutoFire { get; set; }  //Whether holding fire button will make it fire

    //Internals for firerate control

    /// <summary>
    /// Checks if gun is in cool down and creates an instance of GunBullet
    /// </summary>
    /// <param name="cameraAngle">The angle the player is looking at in vector 3</param>
    public virtual void Shoot(Vector3 cameraAngle)
    {

    }


    // Start is called before the first frame update
    public virtual void Start() { }

    // Update is called once per frame
    public virtual void Update(){ }
}



