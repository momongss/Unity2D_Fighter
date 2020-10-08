using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponNormal : Bullet
{
    // Start is called before the first frame update
    private void Awake()
    {
        targetLayer = LAYER_ENEMY;

        damageType = 0;
    }

    private void Update()
    {

    }
}
