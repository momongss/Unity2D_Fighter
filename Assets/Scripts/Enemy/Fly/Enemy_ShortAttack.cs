using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShortAttack : Enemy_Fly
{
    public float damage;
    public int damageType;

    private bool OnAttack;
    private HPObject player;
    private Collider2D _collision;

    protected override void Awake()
    {
        base.Awake();

        OnAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)  // Player 
        {
            OnAttack = true;
            stop = true;
            player = collision.gameObject.GetComponent<HPObject>();
            _collision = collision;
            Attack();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)  // Player
        {
            OnAttack = false;
            stop = false;
        }
    }

    private void Attack()
    {
        if (OnAttack)
        {
            player.attacked(_collision.transform.position, damage, damageType, 0);
            Invoke("Attack", attackSpeed);
        }
    }

    protected override void AttackPlayer()
    {
    }

    protected override void ReleaseIce()
    {
        base.ReleaseIce();
        stop = false;
    }
}
