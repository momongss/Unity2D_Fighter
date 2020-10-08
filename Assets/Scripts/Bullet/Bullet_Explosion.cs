using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Explosion : Bullet
{
    public float ExplosionRange;

    private int DamagedLayer;

    void Awake()
    {
        targetLayer = LAYER_NOTPLAYER;
        DamagedLayer = LAYER_ENEMY;

        // damageType = ;
    }

    void Update()
    {

    }

    protected override void attack(HPObject _, Vector2 collisionDir)
    {
        Collider2D[] target = Physics2D.OverlapCircleAll(transform.position, ExplosionRange);
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i].gameObject.layer == DamagedLayer)
            {
                HPObject nowTarget = target[i].gameObject.GetComponent<HPObject>();
                nowTarget.attacked(collisionDir, damage, damageType, effectDuration);
            }
        }
        Destroy(gameObject); // 총알 삭제

        // 이곳에서 폭발 이펙트 생성
    }
}
