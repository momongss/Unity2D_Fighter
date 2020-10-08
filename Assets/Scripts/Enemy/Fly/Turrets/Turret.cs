using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : HPObject
{
    public GameObject weapon;
    public float DefaultAttackSpeed;
    public float AttackRange;
    public float maxHP;
    public Image healthBarFilled;

    private float attackSpeed;
    private float fireDelay;
    private float HP;
    private bool onShoot;
    private Transform PlayerTransform;
    private Transform Transform;

    protected void Awake()
    {
        healthBarFilled.fillAmount = 1.0f;
        onShoot = false;
        PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
        Transform = GetComponent<Transform>();
        fireDelay = weapon.GetComponent<Bullet>().FireDelay;
        attackSpeed = DefaultAttackSpeed;
        HP = maxHP;
    }

    protected override void OnDamaged(Vector2 collisionDir, float damage, int damageType, float effectDuration)
    {
        // HP
        HP -= damage;
        healthBarFilled.fillAmount = HP / maxHP;
        if (HP <= 0.0f)
            OnDie();
    }

    protected void OnDie()
    {
        Destroy(gameObject);
    }

    protected void FixedUpdate()
    {
        Vector2 playerPosition = PlayerTransform.position;
        Vector2 position = Transform.position;

        float distance = Vector2.Distance(playerPosition, position);
        
        if (distance <= AttackRange)
            AttackPlayer();
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

    void chargeBullet()
    {
        onShoot = false;
    }

    protected void AttackPlayer()
    {
        if (!onShoot)
        {
            onShoot = true;
            OnFire();
        }
    }
}
