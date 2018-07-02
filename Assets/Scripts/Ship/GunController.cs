using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [HideInInspector]
    public Transform muzzle;

    public float fireRate;

    private float coolDown;
    private bool canFire;

    // Use this for initialization
    void Start()
    {
        muzzle = transform.Find("Muzzle");
    }

    // Update is called once per frame
    void Update()
    {
        FirePrimary();
    }

    void FirePrimary() 
    {
        if (InputManager.PrimaryFireButtonDown())
            Debug.Log("Primary Fired");
    }
}
