using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponIce : Bullet
{
    // Start is called before the first frame update
    private void Awake()
    {
        targetLayer = LAYER_ENEMY;
        damageType = TYPE_ICE;
        effectDuration = 3.0f;
    }

    private void Update()
    {

    }
}
