using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float FireDelay; // 연사 속도
    public float Speed; // 투사체 속도

    public const int TYPE_NORMAL = 0;
    public const int TYPE_FIRE = 1;
    public const int TYPE_ICE = 2;
    public const int TYPE_LIGHTNING = 3;
    public const int TYPE_ICE2 = 20;

    protected int targetLayer;
    protected int damageType;
    protected float effectDuration;

    protected const int LAYER_NOTPLAYER = -10;
    protected const int LAYER_PLAYER = 10;
    protected const int LAYER_ENEMY = 11;

    bool HIt;

    private void Awake()
    {
        HIt = false;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (HIt) return;        

        if (collision.gameObject.layer == targetLayer || (targetLayer < 0 && collision.gameObject.layer != -targetLayer))
        {
            HIt = true;
            Destroy(gameObject); // 총알 삭제

            HPObject target = collision.gameObject.GetComponent<HPObject>();
            Vector2 collisionDir = transform.position - collision.gameObject.transform.position;
            collisionDir = collisionDir.normalized;
            attack(target, collisionDir);
            Destroy(gameObject); // 총알 삭제
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (HIt) return;

        if (collision.gameObject.layer == targetLayer || (targetLayer < 0 && collision.gameObject.layer != -targetLayer))
        {
            HIt = true;
            Destroy(gameObject); // 총알 삭제

            HPObject target = collision.gameObject.GetComponent<HPObject>();
            Vector2 collisionDir = transform.position - collision.gameObject.transform.position;
            collisionDir = collisionDir.normalized;
            attack(target, collisionDir);
        }
    }

    protected virtual void attack(HPObject target, Vector2 collisionDir)
    {
        target.attacked(collisionDir, damage, damageType, effectDuration);
    }

    public int getDamageType()
    {
        return damageType;
    }

    public float getEffectDuration()
    {
        return effectDuration;
    }
}