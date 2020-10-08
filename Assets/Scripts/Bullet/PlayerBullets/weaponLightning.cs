using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponLightning : Bullet
{
    // Start is called before the first frame update
    private void Awake()
    {
        targetLayer = LAYER_ENEMY;
        damageType = TYPE_LIGHTNING;
    }

    private void Update()
    {

    }
}
