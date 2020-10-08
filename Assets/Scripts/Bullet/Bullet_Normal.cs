using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Bullet
{
    private void Awake()
    {
        targetLayer = LAYER_ENEMY;

        damageType = TYPE_NORMAL;
    }

    private void Update()
    {

    }
}
