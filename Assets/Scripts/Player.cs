using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public struct PlayerInitStatus
{
    public int moveSpeed;
    public int health;
    public int maxHealth;
    public int atkPower;
    public float atkSpeed;
    public float evasionChance;
    public float projectileSpeed;
    public float knockbackPower;
    public int projectilePenetration;
}


public class Player : Character
{
    public static Player Instance;
    /*
    Character.cs에서 상속받음
    public float moveSpeed = 5f; //이동 속도
    public int health = 2; //체력
    public int atkPower = 2; //공격력
    public float atkSpeed = 1f; //공격 속도
    */

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float moveX, moveY;
    private Coroutine takeDamageRoutine = null;
    //private Coroutine fireRoutine = null;
    [Header("플레이어 속성")]
    public int maxHealth;//플레이어 최대 체력
    public float evasionChance = 0f; //플레이어 회피 확률
    public float projectileSpeed = 10f; //플레이어 투사체 속도
    public float knockbackPower = 1f; //플레이어 투사체 넉백
    public int projectilePenetration = 1; //플레이어 투사체 관통력
    public bool isAlive = true; //플레이어 생존 여부

    [Header("참조")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject auraEffect;

    private int slowEffectCount = 0; // 속도 감소 효과의 개수

    public enum SkillType
    {
        AttackPower,
        MoveSpeed,
        AttackSpeed,
        ProjectileSpeed,
        MaxHealthIncrease
    }

    private void Awake()
    {
        Initiate();
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PoolManager.Instance.CreatePool(projectilePrefab, 10);
        StartCoroutine(FireProjectile());
        //auraEffect.SetActive(false);
    }

    private void Initiate()
    {
        moveSpeed = 5;
        health = 5;
        atkPower = 2;
        atkSpeed = 1;
        maxHealth = health;
        evasionChance = 0;
        projectileSpeed = 10;
        knockbackPower = 1;
        projectilePenetration = 1;
        SetMaxHealth(maxHealth);

        // 각 변수 값을 Debug로 출력
        Debug.Log("Move Speed: " + moveSpeed);
        Debug.Log("Health: " + health);
        Debug.Log("Attack Power: " + atkPower);
        Debug.Log("Attack Speed: " + atkSpeed);
        Debug.Log("Max Health: " + maxHealth);
        Debug.Log("Evasion Chance: " + evasionChance);
        Debug.Log("Projectile Speed: " + projectileSpeed);
        Debug.Log("Knockback Power: " + knockbackPower);
        Debug.Log("Projectile Penetration: " + projectilePenetration);
    }

    private void Start()
    {
        SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        UIManager.Instance.HealthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -0.8f, 0));
        KeyInput();
        if (CanAct())
        {
            Move();//움직임 처리
            SetAnimParams();//애니메이터 파라미터 설정
            ApplyMaxHealthPerkDamage();
        }
        HandleSkillLevelUp();//스킬 레벨업 처리
    }

    public void SetMaxHealth(int maxHp)
    {
        int diff = maxHp - maxHealth;
        maxHealth = maxHp;
        UIManager.Instance.HealthBar.maxValue = maxHealth;
        if (diff > 0)
        {
            health += diff;
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        SetHealth(health);
    }

    private void SetHealth(int hp)
    {
        UIManager.Instance.HealthBar.value = hp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //회복처리
    }

    public override void TakeDamage(int damage)
    {
        if (SkillManager.Instance.Skills[(int)SkillType.MoveSpeed].hasPerk && Random.value * 100 < evasionChance) //이동속도 특전이 있는경우 30퍼센트(임시) 확률로 회피
        {
            Debug.Log("회피 성공");
            //뭔가 이펙트가 있어야할지도..
            return;
        }

        takeDamageRoutine ??= StartCoroutine(TakeDamageRoutine());
        base.TakeDamage(damage);
        SetHealth(health);
    }

    protected override void Death()
    {
        //플레이어 사망처리
        isAlive = false;
        auraEffect.SetActive(false);
        UIManager.Instance.ToggleDeathMessage();
    }

    protected override void Move()
    {
        //플레이어 움직임
        (moveX, moveY) = (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(new Vector2(moveX, moveY).normalized * moveSpeed * Time.deltaTime);
    }

    private void SetAnimParams()
    {
        Vector3 direction = (MouseManager.Instance.transform.position - transform.position).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // 좌우
        {
            animator.SetFloat("dirX", direction.x > 0 ? 1f : -1f);
            animator.SetFloat("dirY", 0f);
        }
        else //상하
        {
            animator.SetFloat("dirY", direction.y > 0 ? 1f : -1f);
            animator.SetFloat("dirX", 0f);
        }
    }

    IEnumerator FireProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(atkSpeed);
            //PlayerProjectile projectile = PoolManager.Instance.Get(projectilePrefab).GetComponent<PlayerProjectile>();
            Vector3 baseDirection = (MouseManager.Instance.mousePos - transform.position).normalized;
            if (SkillManager.Instance.Skills[(int)SkillType.AttackSpeed].hasPerk)//공격 속도 특전, 투사체 개수 증가(좌우로 하나씩 추가)
            {
                float angleOffset = 5f; // 좌우 발사 각도 차이
                for (int i = -1; i <= 1; i++)
                {
                    float angle = i * angleOffset;
                    Vector3 rotatedDirection = Quaternion.Euler(0, 0, angle) * baseDirection;
                    FireProjectile(rotatedDirection);
                }
            }
            else
            {
                FireProjectile(baseDirection);
            }
        }
    }
    void FireProjectile(Vector3 direction)
    {
        PlayerProjectile projectile = PoolManager.Instance.Get(projectilePrefab).GetComponent<PlayerProjectile>();
        projectile.Initialize(this);
        projectile.transform.position = transform.position;
        projectile.direction = direction;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.PlayerThrow);
    }

    IEnumerator TakeDamageRoutine()
    {
        // 원래 색상 저장
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        Color reducedAlphaColor = spriteRenderer.color;
        reducedAlphaColor.a = 0.5f;
        spriteRenderer.color = reducedAlphaColor;

        yield return new WaitForSeconds(0.5f);

        spriteRenderer.color = originalColor;
        takeDamageRoutine = null;
    }

    private void HandleSkillLevelUp()
    {
        if (UIManager.Instance.IsSkillTreeOpen)
        {
            Skill skill = null;
            //추후에 UI로 변경
            if (Input.GetKeyDown(KeyCode.Alpha1)) skill = SkillManager.Instance.Skills[(int)SkillType.AttackPower]; //공격력
            if (Input.GetKeyDown(KeyCode.Alpha2)) skill = SkillManager.Instance.Skills[(int)SkillType.MoveSpeed]; //이동속도
            if (Input.GetKeyDown(KeyCode.Alpha3)) skill = SkillManager.Instance.Skills[(int)SkillType.AttackSpeed]; //공격속도
            if (Input.GetKeyDown(KeyCode.Alpha4)) skill = SkillManager.Instance.Skills[(int)SkillType.ProjectileSpeed]; //투사체 속도
            if (Input.GetKeyDown(KeyCode.Alpha5)) skill = SkillManager.Instance.Skills[(int)SkillType.MaxHealthIncrease]; //최대체력 증가

            if (skill != null)
            {
                SkillManager.Instance.TryLevelUpSkill(skill);
                //StopCoroutine(fireRoutine);
                //fireRoutine = StartCoroutine(FireProjectile());
            }
        }
    }

    private void ApplyMaxHealthPerkDamage()
    {
        if (SkillManager.Instance.Skills[(int)SkillType.MaxHealthIncrease].hasPerk && !auraEffect.activeSelf) // 최대 체력 특전 && auraEffect가 활성화 돼있지 않을 때
        {
            auraEffect.SetActive(true);
            auraEffect.GetComponent<PlayerAura>().SetDamage(5);// 5만큼 데미지 최대체력 x
        }
    }

    private void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!UIManager.Instance.IsEscapeMenuOpen && !UIManager.Instance.IsMenuOpen)
            {
                UIManager.Instance.ToggleEscapeMenu();
            }
            else if (UIManager.Instance.IsMenuOpen)
            {
                UIManager.Instance.ToggleMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (CanAct() || UIManager.Instance.IsSkillTreeOpen)
            {
                UIManager.Instance.ToggleSkillTree();
            }
        }
    }

    private bool CanAct()
    {
        if (GameManager.Instance.IsGamePaused || !isAlive)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ApplySlowEffect(float speedReductionFactor)
    {
        if (slowEffectCount == 0)
        {
            moveSpeed *= speedReductionFactor;
        }
        slowEffectCount++;
        Debug.Log($"ApplySlowEffect: slowEffectCount = {slowEffectCount}");
    }

    public void RemoveSlowEffect(float speedReductionFactor)
    {
        if (slowEffectCount > 0)
        {
            slowEffectCount--;
            if (slowEffectCount == 0)
            {
                moveSpeed /= speedReductionFactor;
            }
            Debug.Log($"RemoveSlowEffect: slowEffectCount = {slowEffectCount}");
        }
    }
}
