using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsGun : MonoBehaviour
{
  
    public GameObject GunBullet; //Bullet that will be shot

    //Stats for any kind of weapon
    public float FireRate;   //Unit in rounds per minute
    public float ReloadTime; //Unit in seconds
    public int MaxAmmo;
    [HideInInspector] public int CurrentAmmo;

    public bool isAutoFire { get; set; }  //Whether holding fire button will make it fire

    //Internals for firerate control

    /// <summary>
    /// Checks if gun is in cool down and creates an instance of GunBullet
    /// </summary>
    /// <param name="cameraAngle"> The angle the player is looking at in vector 3 </param>
    public virtual void Shoot(Vector3 cameraAngle)
    {

    }

    // Start is called before the first frame update
    public virtual void Start() { }

    // Update is called once per frame
    public virtual void Update(){ }

}
