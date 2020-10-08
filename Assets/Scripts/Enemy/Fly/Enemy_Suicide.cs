using UnityEngine;

public class Enemy_Suicide : Enemy_Fly
{
    public float damage;
    public int damageType;

    public float dashSpeed;
    public float dashCoolTime;
    public float dashDelay;

    private Vector2 dashPoint;
    private bool onDash;
    private bool onDashing;

    protected override void Awake()
    {
        base.Awake();

        onDash = true;
        onDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)  // Player 
        {
            HPObject player = collision.gameObject.GetComponent<HPObject>();
            player.attacked(collision.transform.position, damage, damageType, 0);

            OnDie(); // 폭발모션 추가해야 함
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!PlayerDetected) return;

        Vector2 position = Transform.position;

        // 대쉬 종료
        if (Vector2.Distance(position, dashPoint) < 0.3)
        {
            onDashing = false;
            stop = false;

            float attackspeed = attackSpeed;
            if (attackSpeed == 0)
                attackspeed = 0.01f;

            float cooltime = (1 / attackspeed) * dashCoolTime;
            Invoke("chargeDash", cooltime); // 대쉬 쿨타임
        }

        // 대쉬중
        if (onDashing)
        {
            Vector2 dashDirc = dashPoint - position;
            rigid.velocity = dashDirc * dashSpeed;

            if (rigid.velocity.x > 0) headDirc = 1;
            if (rigid.velocity.x < 0) headDirc = -1;

            // 애니메이션 처리
            anim.SetInteger("WalkSpeed", headDirc);
            if (headDirc > 0) spriteRenderer.flipX = true;
            if (headDirc < 0) spriteRenderer.flipX = false;
        }
    }

    protected override void AttackPlayer()
    {
        dashPoint = PlayerTransform.position;

        if (onDash)
        {
            onDash = false;
            stop = true;

            float attackspeed = attackSpeed;
            if (attackSpeed == 0)
                attackspeed = 0.01f;

            float delay = (1 / attackspeed) * dashDelay;
            Invoke("Dash", delay); // 대쉬 준비시간
        }
    }

    protected override void ReleaseIce()
    {
        base.ReleaseIce();
        onDash = true;
        onDashing = false;
        stop = false;
    }

    private void Dash()
    {
        onDashing = true;
    }
    
    private void chargeDash()
    {
        onDash = true;
    }
}
