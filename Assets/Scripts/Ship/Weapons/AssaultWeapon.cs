using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultWeapon : GunController {

    public override void Fire()
    {
        base.Fire();

        if (shouldFire) {
            // Fire gun
        }
            
    }
}
