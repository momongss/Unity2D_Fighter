using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : HPObject
{
    public float maxSpeed;
    public float minSpeed;
    public float Accelation;

    public WeaponManager weaponManager;
    public Image healthBarFilled;
    public Image manaBarFilled;
    public Image[] weaponIcon;
    public Image[] skillIcon;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Camera mainCamera;
    PlatformFighter platformfighter;

    PlayerInfo playerInfo;
    WeaponInfo weaponInfo;

    private float HP;
    private float MP;
    private float maxHP;
    private float maxMP;

    private bool isBullet;

    private List<SkillInfo> skillInfo;
    private List<Timer> skillCoolTimer;
    private List<Timer> skillDuration;
    private List<bool> skillOn;

    // 스킬이름과 ID에 맞춰 설정
    private const int SPREAD_ATTACK = 0;
    private const int FULL_ATTACK = 3;
    private const int ICE_FULL_ATTACK = 7;
    private const int FLASH = 9;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        playerInfo = platformfighter.getPlayerInfo();
        weaponInfo = platformfighter.getWeaponInfo();
        skillInfo = platformfighter.getSkillList();

        isBullet = true;

        maxHP = playerInfo.maxHP;
        maxMP = playerInfo.maxMP;

        HP = maxHP;
        MP = maxMP;

        skillCoolTimer = new List<Timer>();
        skillDuration = new List<Timer>();
        skillOn = new List<bool>();
        for (int i = 0; i < 12; i++)
        {
            float F_coolTime = skillInfo[i].CoolTime + skillInfo[i].UpCoolTime * (weaponInfo.weaponLevels[i / 3] - 1);
            long L_coolTime = (long)(F_coolTime * 1000);
            skillCoolTimer.Add(new Timer(L_coolTime));

            if (skillInfo[i].Option.ContainsKey("Duration"))
            {
                SkillInfo.SkillOption option = skillInfo[i].Option["Duration"];
                float F_Duration = option.InitialValue + option.Upgrade * (weaponInfo.weaponLevels[i / 3] - 1);
                long L_Duration = (long)(F_Duration * 1000);
                skillDuration.Add(new Timer(L_Duration));
            }
            else
            {
                skillDuration.Add(new Timer(0));
            }

            skillOn.Add(false);
        }
    }

    private void Update()
    {
        int attackbuffcheck = -1;
        for (int i = 0; i < 12; i++)
        {
            if (skillOn[i] && skillInfo[i].SkillType == SkillInfo.ATTACK_BUFF)
            {
                attackbuffcheck = i;
                break;
            }
        }

        if (attackbuffcheck == SPREAD_ATTACK)
        {
            SpreadAttack();
        }
        else
        {
            AttackNormal();
        }
        AttackSkill();

        // 마우스 우클릭 (무기 전환)
        if (Input.GetMouseButtonDown(1))
        {
            weaponManager.nextWeapon();
        }

        // 방향 바꾸기
        if (rigid.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rigid.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Animation
        if (rigid.velocity.normalized.x == 0)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        // WeaponUI
        int usingweapon = weaponManager.getUsingWeapon();
        for (int i = 0; i < 4; i++)
        {
            weaponIcon[i].color = i == usingweapon ? Color.green : Color.red;
        }
        for (int i = 0; i < 3; i++)
        {
            int skillID = 3 * usingweapon + i;
            float coolTime = skillCoolTimer[skillID].getTime();
            float fullcoolTime = skillCoolTimer[skillID].getWaitTime();
            float fillAmount = coolTime / fullcoolTime;
            if (fillAmount > 1.0f) fillAmount = 1.0f;
            if (fillAmount == 1.0f) skillIcon[i].color = Color.green;
            else skillIcon[i].color = Color.red;
            skillIcon[i].fillAmount = fillAmount;
        }
    }

    private void FixedUpdate()
    {
        // 가속
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.up * v * Accelation, ForceMode2D.Impulse);
        rigid.AddForce(Vector2.right * h * Accelation, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        if (rigid.velocity.y > maxSpeed)
            rigid.velocity = new Vector2(rigid.velocity.x, maxSpeed);
        else if (rigid.velocity.y < -maxSpeed)
            rigid.velocity = new Vector2(rigid.velocity.x, -maxSpeed);

        // 감속
        if (!Input.GetButton("Horizontal") && (Mathf.Abs(rigid.velocity.x) < minSpeed))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        if (!Input.GetButton("Vertical") && (Mathf.Abs(rigid.velocity.y) < minSpeed))
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    protected override void OnDamaged(Vector2 targetPos, float damage, int damageType, float effectDuration)
    {
        float MaxDefensiveRate = 0f;

        for (int i = 0; i < 12; i++)
        {
            if (skillOn[i] && skillInfo[i].SkillType == SkillInfo.DEFENSIVE_BUFF)
            {
                int weapon = i / 3;
                float DefensiveRate = skillInfo[i].Power + skillInfo[i].UpPower * (weaponInfo.weaponLevels[weapon] - 1);
                if (MaxDefensiveRate < DefensiveRate) MaxDefensiveRate = DefensiveRate;
            }
        }

        if (MaxDefensiveRate > 1) MaxDefensiveRate = 1f;

        float finalDamage = damage * (1 - MaxDefensiveRate);

        // 애니메이션 처리
        animator.SetTrigger("doDamaged");

        // 체력, 체력바
        HP -= finalDamage;
        healthBarFilled.fillAmount = HP / maxHP;
        if (HP <= 0)
            OnDie();

    }

    public void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y
        spriteRenderer.flipY = true;
        // Collider Disable
        GetComponent<CapsuleCollider2D>().enabled = false;
        // Die Effect Jump
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);

        // 부활, 목숨없으면 겜종료
    }

    private void chargeBullet()
    {
        isBullet = true;
    }

    private void AttackNormal()
    {
        // 마우스 좌클릭 (무기 사용)
        if (Input.GetMouseButton(0))
        {
            if (isBullet)
            {
                // 사용중인 무기 가져오기
                GameObject weapon = weaponManager.getWeapon();

                // 총알생성위치 지정
                weapon.transform.position = transform.position;

                if (weaponManager.getUsingWeapon() == 3)    // 번개 : 3
                {
                    Debug.Log("번개 시작");
                    GameObject[] EnemySets = GameObject.FindGameObjectsWithTag("Enemy");
                    GameObject nearEnemy = null;

                    float minDistance = -1;
                    float threshold = 3;
                    Vector2 MousePosition = Input.mousePosition;
                    MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);

                    // for (int i = 0; i < EnemySets.transform.childCount; i++)
                    foreach (GameObject enemy in EnemySets)
                    {
                        // GameObject enemy = EnemySets.transform.GetChild(i).gameObject;
                        float distance = Vector2.Distance(MousePosition, enemy.transform.position);
                        if (minDistance == -1 || distance < minDistance)
                        {
                            minDistance = distance;
                            nearEnemy = enemy;
                        }
                    }

                    Debug.Log("거리 : " + minDistance);
                    // 타겟 지정된 적이 있을 때 && 적이 충분히 가까울 때 데미지 가함
                    if (0 <= minDistance && minDistance < threshold)
                    {
                        Vector2 collisionDir = transform.position - nearEnemy.transform.position;
                        float damage = weapon.GetComponent<Bullet>().damage;
                        int damageType = weapon.GetComponent<Bullet>().getDamageType();
                        float effectDuration = weapon.GetComponent<Bullet>().getEffectDuration();
                        float fireDelay = weapon.GetComponent<Bullet>().FireDelay;
                        nearEnemy.GetComponent<HPObject>().attacked(collisionDir, damage, damageType, effectDuration);

                        isBullet = false;
                        Invoke("chargeBullet", fireDelay);
                    }
                }
                else
                {
                    // 총알발사 방향 계산
                    Vector3 MousePosition = Input.mousePosition;
                    MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);
                    Vector2 direction = MousePosition - transform.position;
                    direction = direction.normalized;

                    // 총알 생성, 생성된 총알 이동
                    GameObject bullet = Instantiate(weapon);
                    float fireDelay = bullet.GetComponent<Bullet>().FireDelay;
                    float bulletSpeed = bullet.GetComponent<Bullet>().Speed;
                    Rigidbody2D bullet_rigid = bullet.GetComponent<Rigidbody2D>();
                    bullet_rigid.velocity = direction * bulletSpeed;

                    isBullet = false;
                    Invoke("chargeBullet", fireDelay);
                }
            }
        }
    }

    private void AttackSkill()
    {
        bool key1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool key2 = Input.GetKeyDown(KeyCode.Alpha2);
        bool key3 = Input.GetKeyDown(KeyCode.Alpha3);

        // 키보드 1, 2, 3 입력
        if (key1 || key2 || key3)
        {
            int usingweapon = weaponManager.getUsingWeapon();
            int skillNumber = 00;
            if (key1)
                skillNumber = 00;
            else if (key2)
                skillNumber = 01;
            else if (key3)
                skillNumber = 02;

            int skillID = 3 * usingweapon + skillNumber;

            SkillInfo skillinfo = skillInfo[skillID];
            int skillType = skillinfo.SkillType;
            float manaConsumption = skillinfo.ManaConsumption + skillinfo.UpManaConsumption * (weaponInfo.weaponLevels[usingweapon] - 1);

            // 남은 마나량, 쿨타임 확인
            if (MP >= manaConsumption && skillCoolTimer[skillID].isDone())
            {
                if (skillType == SkillInfo.ATTACK_BUFF)
                {
                    // 마나소모
                    MP -= manaConsumption;
                    manaBarFilled.fillAmount = MP / maxMP;

                    // 쿨타임 적용
                    skillCoolTimer[skillID].setTime();

                    // 스킬 지속시간 초기화
                    skillDuration[skillID].setTime();
                }
                else if (skillType == SkillInfo.ATTACK)
                {
                    // 마나소모
                    MP -= manaConsumption;
                    manaBarFilled.fillAmount = MP / maxMP;

                    // 쿨타임 적용
                    skillCoolTimer[skillID].setTime();

                    // 공격 적용
                    if (skillID == FULL_ATTACK)
                    {
                        FullAttack();
                    }
                    else if (skillID == ICE_FULL_ATTACK)
                    {
                        IceFullAttack();
                    }
                }
                else if (skillType == SkillInfo.DEFENSIVE_BUFF)
                {
                    // 마나소모
                    MP -= manaConsumption;
                    manaBarFilled.fillAmount = MP / maxMP;

                    // 쿨타임 적용
                    skillCoolTimer[skillID].setTime();

                    // 스킬 지속시간 초기화
                    skillDuration[skillID].setTime();
                }
                else if (skillType == SkillInfo.MOVE)
                {
                    // 마나소모
                    MP -= manaConsumption;
                    manaBarFilled.fillAmount = MP / maxMP;

                    // 쿨타임 적용
                    skillCoolTimer[skillID].setTime();

                    if (skillID == FLASH)
                    {
                        Flash();
                    }
                }
            }
        }

        // 스킬 지속시간 체크하여 스킬 온/오프
        for (int i = 0; i < 12; i++)
        {
            skillOn[i] = !skillDuration[i].isDone();
        }
    }

    private void SpreadAttack()
    {
        // 마우스 좌클릭 (무기 사용)
        if (Input.GetMouseButton(0))
        {
            if (isBullet)
            {
                // 사용중인 무기 가져오기
                GameObject weapon = weaponManager.getWeapon();

                // 총알생성위치 지정
                weapon.transform.position = transform.position;

                // 총알발사 방향 계산
                Vector3 MousePosition = Input.mousePosition;
                MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);
                Vector2 direction = MousePosition - transform.position;
                direction = direction.normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float angleDown = (angle - 30.0f < -180.0f ? angle + 330.0f : angle - 30.0f) * Mathf.Deg2Rad;
                float angleUp = (angle + 30.0f > 180.0f ? angle - 330.0f : angle + 30.0f) * Mathf.Deg2Rad;

                Vector2 directionDown = new Vector2(Mathf.Cos(angleDown), Mathf.Sin(angleDown));
                Vector2 directionUp = new Vector2(Mathf.Cos(angleUp), Mathf.Sin(angleUp));

                // 총알 생성, 생성된 총알 이동
                GameObject bullet = Instantiate(weapon);
                float fireDelay = bullet.GetComponent<Bullet>().FireDelay;
                float bulletSpeed = bullet.GetComponent<Bullet>().Speed;
                Rigidbody2D bullet_rigid = bullet.GetComponent<Rigidbody2D>();
                bullet_rigid.velocity = direction * bulletSpeed;

                GameObject bulletUp = Instantiate(weapon);
                fireDelay = bulletUp.GetComponent<Bullet>().FireDelay;
                bulletSpeed = bulletUp.GetComponent<Bullet>().Speed;
                bullet_rigid = bulletUp.GetComponent<Rigidbody2D>();
                bullet_rigid.velocity = directionUp * bulletSpeed;

                GameObject bulletDown = Instantiate(weapon);
                fireDelay = bulletDown.GetComponent<Bullet>().FireDelay;
                bulletSpeed = bulletDown.GetComponent<Bullet>().Speed;
                bullet_rigid = bulletDown.GetComponent<Rigidbody2D>();
                bullet_rigid.velocity = directionDown * bulletSpeed;

                isBullet = false;
                Invoke("chargeBullet", fireDelay);
            }
        }
    }

    private void FullAttack()
    {
        SkillInfo fullAttack = skillInfo[FULL_ATTACK];
        int weapon = FULL_ATTACK / 3;
        float attackRange = fullAttack.Option["Range"].InitialValue + fullAttack.Option["Range"].Upgrade * (weaponInfo.weaponLevels[weapon] - 1);
        float damage = fullAttack.Power + fullAttack.UpPower * (weaponInfo.weaponLevels[weapon] - 1);
        int damageType = (int)fullAttack.Option["DamageType"].InitialValue;

        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");

        Vector2 playerPosition = transform.position;

        for (int i = 0; i < enemy.Length; i++)
        {
            Vector2 enemyPosition = enemy[i].transform.position;
            float distance = Vector2.Distance(playerPosition, enemyPosition);
            if (distance <= attackRange)
            {
                enemy[i].GetComponent<HPObject>().attacked(playerPosition - enemyPosition, damage, damageType, 2);
            }
        }
    }

    private void IceFullAttack()
    {
        SkillInfo fullAttack = skillInfo[ICE_FULL_ATTACK];
        int weapon = ICE_FULL_ATTACK / 3;
        float attackRange = fullAttack.Option["Range"].InitialValue + fullAttack.Option["Range"].Upgrade * (weaponInfo.weaponLevels[weapon] - 1);
        float damage = fullAttack.Power + fullAttack.UpPower * (weaponInfo.weaponLevels[weapon] - 1);
        int damageType = (int)fullAttack.Option["DamageType"].InitialValue;

        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");

        Vector2 playerPosition = transform.position;

        for (int i = 0; i < enemy.Length; i++)
        {
            Vector2 enemyPosition = enemy[i].transform.position;
            float distance = Vector2.Distance(playerPosition, enemyPosition);
            if (distance <= attackRange)
            {
                enemy[i].GetComponent<HPObject>().attacked(playerPosition - enemyPosition, damage, damageType, 5);
            }
        }
    }

    private void Flash()
    {
        SkillInfo flash = skillInfo[FLASH];
        int weapon = FLASH / 3;
        float range = flash.Option["Range"].InitialValue + flash.Option["Range"].Upgrade * (weaponInfo.weaponLevels[weapon] - 1);

        Vector2 MousePosition = Input.mousePosition;
        MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);

        float dist = Vector2.Distance(MousePosition, transform.position);

        if (dist <= range)
        {
            transform.position = MousePosition;
        }
        else
        {
            Vector2 PlayerPosition = transform.position;
            Vector2 direction = (MousePosition - PlayerPosition).normalized;
            transform.position = PlayerPosition + direction * range;
        }
    }
}
