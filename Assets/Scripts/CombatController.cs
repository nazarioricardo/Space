﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

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