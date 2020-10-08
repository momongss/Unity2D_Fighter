using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponFire : Bullet
{
    HashSet<HPObject> preTarget;

    private const float Destroytime = 0.5f;

    // Start is called before the first frame update
    private void Awake()
    {
        targetLayer = LAYER_ENEMY;
        damageType = TYPE_FIRE;
        preTarget = new HashSet<HPObject>();

        Invoke("DestroyFire", Destroytime);
    }

    private void DestroyFire()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 collisionDir = transform.position - collision.gameObject.transform.position;
        collisionDir = collisionDir.normalized;

        if (collision.gameObject.layer == targetLayer || (targetLayer < 0 && collision.gameObject.layer != -targetLayer))
        {
            HPObject target = collision.gameObject.GetComponent<HPObject>();
            // 이전에 공격한 타겟은 제외
            if (preTarget.Contains(target))
                return;
            preTarget.Add(target);

            // 타겟 공격
            attack(target, collisionDir);
        }
    }

    private void Update()
    {

    }
}
