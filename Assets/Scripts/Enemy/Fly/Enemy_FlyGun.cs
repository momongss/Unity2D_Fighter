using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FlyGun : Enemy_Fly
{
    public GameObject weapon;

    private float fireDelay;
    private bool onShoot;

    protected override void Awake()
    {
        base.Awake();

        onShoot = false;

        fireDelay = weapon.GetComponent<Bullet>().FireDelay;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!PlayerDetected) return;
    }

    void OnFire()
    {
        Vector2 playerPosition = PlayerTransform.position;
        Vector2 position = Transform.position;

        // 총알생성위치 지정
        weapon.transform.position = position;

        // 총알발사 방향 계산
        Vector2 direction = (playerPosition - position).normalized;

        // 총알 생성, 생성된 총알 이동
        GameObject go = Instantiate(weapon);
        Rigidbody2D bullet_rigid = go.GetComponent<Rigidbody2D>();
        bullet_rigid.velocity = direction * weapon.GetComponent<Bullet>().Speed;

        float attackspeed = attackSpeed;
        if (attackSpeed == 0)
            attackspeed = 0.01f;

        float delay = (1 / attackspeed) * fireDelay;

        Invoke("chargeBullet", delay);
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
            OnFire();
        }
    }
}
