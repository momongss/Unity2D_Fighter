using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WarMachine : Enemy_Fly
{
    public GameObject weapon;

    private float fireDelay;
    private bool onShoot;

    protected override void Awake()
    {
        base.Awake();

        onShoot = false;
    }

    protected override void ReleaseIce()
    {
        base.ReleaseIce();
        chargeBullet();
    }

    void chargeBullet()
    {
        onShoot = false;
    }

    protected override void AttackPlayer()
    {
        if (!onShoot)
        {
            onShoot = true;
        }
    }
}