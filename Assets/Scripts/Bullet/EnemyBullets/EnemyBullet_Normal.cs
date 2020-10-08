using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet_Normal : Bullet
{
    private void Awake()
    {
        targetLayer = LAYER_PLAYER;
    }

    private void Update()
    {

    }
}
