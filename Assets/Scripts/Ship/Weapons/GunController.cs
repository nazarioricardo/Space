using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [HideInInspector]
    public Transform muzzle;

    [HideInInspector]
    public bool shouldFire;

    public Projectile projectile;
    public float fireRate;

    private float nextShotTime;

    // Use this for initialization
    void Start()
    {
        muzzle = transform.Find("Muzzle");
    }

    public virtual void Fire() 
    {
        shouldFire = false;

        if (Time.time < nextShotTime)
            return;

        nextShotTime = Time.time + fireRate;

        // Instantiate Projectile
        Instantiate(projectile, muzzle.position, muzzle.rotation, gameObject.transform.root);

        shouldFire = true;
    }
}
