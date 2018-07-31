using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsController : MonoBehaviour {

    public GunController primary;

    [HideInInspector]
    public GameObject hull;

    [HideInInspector]
    public InputManager InputManager;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        FirePrimary();
	}

    void FirePrimary() {
        if (InputManager.PrimaryFireButtonDown())
            primary.Fire();
    }
}
