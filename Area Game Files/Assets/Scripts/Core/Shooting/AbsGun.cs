using UnityEngine;

public abstract class AbsGun : MonoBehaviour
{
    public GunStats stats;
    public LayerMask mask;

    public int ammo;
    public float period; // Cooldown time
    public float cooldown; // Cooldown timer
    public float reloading; // Reload timer

    protected Transform cam;
    protected NetworkTag networkTag;

    public virtual bool OnCooldown => cooldown > 0f;
    public virtual bool Reloading => reloading > 0f;

    public void SetCam(Transform cam)
    {
        this.cam = cam;
    }

    private void Start()
    {
        ammo = stats.MaxAmmo;
        cooldown = 0f;
        reloading = 0f;
        networkTag = GetComponent<NetworkTag>();
    }

    protected virtual void Update ()
    {
        if (OnCooldown) cooldown -= Time.deltaTime;

        if (Reloading)
        {
            reloading -= Time.deltaTime;

            if (!Reloading)
            {
                ammo = stats.MaxAmmo;
            }

            else return;
        }
        
        if (Input.GetKey(KeyCode.R))
        {
            Reload();
            return;
        }

        if (stats.IsAutoFire)
        {
            if (Input.GetMouseButton(0)) Shoot();
        }

        else if (Input.GetMouseButtonDown(0)) Shoot();
    }

    /// <summary>
    /// Checks if gun is in cool down and fires
    /// </summary>
    public virtual void Shoot()
    {
        if (cooldown <= 0f && ammo > 0)
        {
            period = 1f / stats.FireRate;
            ammo -= stats.AmmoPerShot;
            cooldown = period;

            Fire();
        }
    }

    /// <summary>
    /// Actual firing of the gun
    /// </summary>
    public virtual void Fire()
    {
        Debug.Log("Fire");
    }

    /// <summary>
    /// Starts the timer to reload
    /// </summary>
    public virtual void Reload()
    {
        reloading = stats.ReloadTime;
    }
}

// Extendable class with inheritance
[System.Serializable]
public class GunStats
{
    public int AmmoPerShot = 1;
    public int MaxAmmo = 10;
    public float FireRate = 10f;   // In rounds per second
    public float ReloadTime = 1f; // In seconds
    public bool IsAutoFire = false;
}

// [SerializeField] This would be usefull for every projectilebased gun. Not every gun though.
// protected GameObject GunBulletPrefab; //Bullet that will be shot 