using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Enemy : HPObject
{
    public int killPoint;
    public float toughness;

    public float DefaultNormalSpeed;
    public float DefaultAttackSpeed;
    public float AttackRange;
    public float RangeFix;

    protected int headDirc;
    protected int IceHItCount;

    public float maxHP;
    public Image healthBarFilled;

    protected float HP;
    protected float normalSpeed;
    protected float attackSpeed;

    protected bool stop;
    protected bool isDie;
    protected bool onHit;
    protected bool PlayerDetected;

    protected Rigidbody2D rigid;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Transform PlayerTransform;
    protected Transform Transform;
    protected StageManager stageManager;

    private List<bool> DebuffOn;
    private List<Timer> DebuffTimer;

    private const int DEBUFF_SIZE = 2;

    private const int DEBUFF_ICE = 0;
    private const int DEBUFF_ICE2 = 1;

    private int TEST_lastAttack = 0;

    protected virtual void Awake()
    {
        healthBarFilled.fillAmount = 1.0f;
        HP = maxHP;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        onHit = false;
        stop = false;
        isDie = false;
        PlayerDetected = true;

        normalSpeed = DefaultNormalSpeed;
        attackSpeed = DefaultAttackSpeed;

        Transform = GetComponent<Transform>();
        PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();

        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();

        DebuffOn = new List<bool>();
        DebuffTimer = new List<Timer>();
        for (int i = 0; i < DEBUFF_SIZE; i++)
        {
            DebuffOn.Add(false);
            DebuffTimer.Add(new Timer(1000));
        }
    }

    protected virtual void FixedUpdate()
    {
        // PlayerDetected = DectectPlayer();

        if (isDie) return;
        if (!PlayerDetected) return;

        Vector2 playerPosition = PlayerTransform.position;
        Vector2 position = Transform.position;

        // 타격을 받은 상태가 아니라면
        if (!onHit && !stop)
        {
            float distance = Vector2.Distance(playerPosition, position);
            if (distance > AttackRange * RangeFix)
            {
                // 이동
                setVelocity(playerPosition, position);

                // 애니메이션
                int nextMove = rigid.velocity.x > 0 ? 1 : -1;
                anim.SetInteger("WalkSpeed", nextMove);
                spriteRenderer.flipX = nextMove > 0;
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }

            if (distance <= AttackRange) AttackPlayer();
        }

        string str = "";

        // 지속시간 계산하여 디버프 제거
        for (int i = 0; i < DEBUFF_SIZE; i++)
        {
            if (DebuffOn[i]) str += "On ";
            else str += "Off ";

            if (DebuffOn[i] && DebuffTimer[i].isDone())
            {
                DebuffOn[i] = false;
                applyDebuff();
            }
        }

        for (int i = 0; i < DEBUFF_SIZE; i++)
        {
            str += DebuffTimer[i].getRemainderTime() + " ";
        }

        str += IceHItCount + " type" + TEST_lastAttack;
    }

    protected void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y
        spriteRenderer.flipY = true;
        // Collider Disable
        GetComponent<CapsuleCollider2D>().enabled = false;
        // Die Effect Jump
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);

        isDie = true;

        DelMonster(1f);
    }

    protected override void OnDamaged(Vector2 collisionDir, float damage, int damageType, float effectDuration)
    {
        TEST_lastAttack = damageType;

        // 죽은 상태인지 확인
        if (isDie) return;

        // 상태 변경
        onHit = true;

        // HP
        HP -= damage;
        healthBarFilled.fillAmount = HP / maxHP;
        if (HP <= 0.0f)
            OnDie();

        // Animation
        spriteRenderer.color = new Color(1, 0.5f, 0.5f, 1);
        anim.SetBool("isDamaged", true);
        Invoke("OffDamaged", 0.2f);

        // 디버프 타이머 처리
        if (damageType == Bullet.TYPE_ICE)
        {
            if (IceHItCount < 5)
                IceHItCount++;

            long debuffDuration = (long)(effectDuration * (100f - toughness) * 10f);
            long remainderTime = DebuffTimer[DEBUFF_ICE].getRemainderTime();
            if (remainderTime < debuffDuration)
            {
                DebuffTimer[DEBUFF_ICE].setWaitTime(debuffDuration);
                DebuffTimer[DEBUFF_ICE].setTime();
            }
            DebuffOn[DEBUFF_ICE] = true;
        }
        else if (damageType == Bullet.TYPE_ICE2)
        {
            long debuffDuration = (long)(effectDuration * (100f - toughness) * 10f);
            long remainderTime = DebuffTimer[DEBUFF_ICE2].getRemainderTime();
            if (remainderTime < debuffDuration)
            {
                DebuffTimer[DEBUFF_ICE2].setWaitTime(debuffDuration);
                DebuffTimer[DEBUFF_ICE2].setTime();
            }
            DebuffOn[DEBUFF_ICE2] = true;
        }

        //디버프 효과 처리
        applyDebuff();
    }

    protected virtual void ReleaseIce()
    {
        IceHItCount = 0;
        normalSpeed = DefaultNormalSpeed;
        attackSpeed = DefaultAttackSpeed;
    }

    protected void applyDebuff()
    {
        if (!DebuffTimer[DEBUFF_ICE2].isDone())
        {
            normalSpeed = 0;
            attackSpeed = 0;
        }
        else if (!DebuffTimer[DEBUFF_ICE].isDone())
        {
            if (IceHItCount == 5)
            {
                normalSpeed = 0;
                attackSpeed = 0;
            }
            else
            {
                normalSpeed = DefaultNormalSpeed * (100f - 15 * IceHItCount) / 100f;
                attackSpeed = DefaultAttackSpeed * (100f - 15 * IceHItCount) / 100f;
            }
        }
        else
        {
            ReleaseIce();
        }
    }

    protected void OffDamaged()
    {
        onHit = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        anim.SetBool("isDamaged", false);
    }

    protected virtual void AttackPlayer()
    {
        // virtual
    }

    protected virtual void setVelocity(Vector2 playerPosition, Vector2 position)
    {
        // 기본 이동 정의, 하위 클래스에서 재정의 가능
        Vector2 moveDirc = (playerPosition - position).normalized;
        rigid.velocity = moveDirc * normalSpeed;
    }

    private bool DectectPlayer()
    {
        float distance = Vector2.Distance(gameObject.transform.position, PlayerTransform.transform.position);
        if (distance < 17)
            return true;
        return false;
    }

    protected void DelMonster(float delay)
    {
        Invoke("_DelMonster", delay);
    }

    private void _DelMonster()
    {
        stageManager.EnemyKill(gameObject);
        Destroy(gameObject);
    }
}