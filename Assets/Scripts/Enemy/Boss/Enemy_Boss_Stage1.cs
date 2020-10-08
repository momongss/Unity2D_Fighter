using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_Stage1 : Enemy_Boss
{
    public GameObject weapon;

    private bool onShoot;
    private const int phaseDelay = 7, bulletSpread = 6;
    private const float delay = 0.1f;
    private int FireCount;
    private List<Vector2> EveryDirections;

    protected override void Awake()
    {
        base.Awake();

        onShoot = false;
        FireCount = 0;
        EveryDirections = new List<Vector2>();
        EveryDirections.Add(new Vector2(-1, -1));
        EveryDirections.Add(new Vector2(-1, 0));
        EveryDirections.Add(new Vector2(-1, 1));
        EveryDirections.Add(new Vector2(0, -1));
        EveryDirections.Add(new Vector2(0, 1));
        EveryDirections.Add(new Vector2(1, -1));
        EveryDirections.Add(new Vector2(1, 0));
        EveryDirections.Add(new Vector2(1, 1));

        Invoke("OnFire", weapon.GetComponent<Bullet>().FireDelay);
    }

    void OnFire()
    {
        if (onShoot) return;
        // 총알생성위치 지정
        weapon.transform.position = transform.position;

        foreach (Vector2 dirc in EveryDirections)
        {
            // 총알 생성, 생성된 총알 이동
            GameObject go = Instantiate(weapon);
            Rigidbody2D bullet_rigid = go.GetComponent<Rigidbody2D>();
            bullet_rigid.velocity = dirc * weapon.GetComponent<Bullet>().Speed;
        }

        FireCount += 1;
        if (FireCount < bulletSpread)
            Invoke("OnFire", delay);
        else
        {
            Invoke("OnFire", phaseDelay);
            FireCount = 0;
        }
    }
}
