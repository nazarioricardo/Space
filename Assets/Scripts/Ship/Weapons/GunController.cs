using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GunController : MonoBehaviour {

    [HideInInspector]
    public Transform muzzle;

    [HideInInspector]
    public List<Transform> muzzles;

    [HideInInspector]
    public bool shouldFire;

    public Projectile projectile;
    public float fireRate;

    private float nextShotTime;

    // Use this for initialization
    void Start()
    {
        muzzle = transform.Find("Muzzle");
        muzzles = GetComponentsInChildren<Transform>().Where(transform => transform.name == "Muzzle").ToList<Transform>();
    }

    public virtual void Fire() 
    {
        shouldFire = false;

        if (Time.time < nextShotTime)
            return;

        nextShotTime = Time.time + fireRate;

        // Instantiate Projectile 
        for (int i = 0; i < muzzles.Count; i++)
        {
            Instantiate(projectile, muzzles[i].position, muzzles[i].rotation, gameObject.transform.root);
        }

        shouldFire = true;
    }
}
