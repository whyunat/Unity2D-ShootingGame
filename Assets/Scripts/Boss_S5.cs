using System.Collections;

using UnityEngine;


public class Boss_S5 : Monster
{
    
    public GameObject projectileObject; // 발사할 투사체 프리팹
    public GameObject projectileObject2; // 발사할 투사체 프리팹2

    public int projectileCount = 15; // 투사체 개수
    public float projectileSpeed = 8f; // 투사체1 속도
    public float projectileSpeed2 = 2f; // 투사체2 속도

    public float fireInterval = 3f; // 발사 주기 (3초)
    public float balanceAngle = 10f; // 발사할 때마다 늘릴 각도 (10도)

    public Transform launcher; // 발사 위치 런처

    public float dieTime = 3f;
    private bool prevIsMonsterRight; // 이전 상태 저장 (런처 좌우 조정)
    private bool isMonsterRight = true; // 기본 런쳐 위치
    private float launcherOffset = 3.2f; // 런쳐 보스에서 떨어진 정도



    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 5초마다 투사체 발사 시작
        StartCoroutine(FireProjectilesRoutine());
    }

    protected override void Death()
    {
        if(!isDeath)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.FireDragonDie);
        }  
        animator.SetBool("isDeath", true);
        isDeath = true;
        StartCoroutine(ReturnToPoolAfterDelay(dieTime)); // ~초후 풀반환


    }

    // 주기마다 투사체 발사 코루틴
    private IEnumerator FireProjectilesRoutine()
    {
        while (true) // 보스가 살아있는 동안 반복
        {
            if (!isDeath) // 죽지 않았을 때만 발사
            {
                FireProjectiles();
                balanceAngle += 5f;
            }
            yield return new WaitForSeconds(fireInterval); // fireInterval만큼 대기
        }
    }


    // delay 후 풀 반환 코루틴
    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.Return(gameObject);
    }

    void Update()
    {
        UpdateLauncherPosition();
        Move();
        Attack();     
    }

    private void UpdateLauncherPosition()
    {
        isMonsterRight = transform.position.x > player.transform.position.x;

        // 상태가 바뀌었을 때만 위치 업데이트
        if (isMonsterRight != prevIsMonsterRight)
        {
            float xOffset = isMonsterRight ? -launcherOffset : launcherOffset;
            launcher.position = transform.position + new Vector3(xOffset, -0.9f, 0);
            prevIsMonsterRight = isMonsterRight;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
    protected override void Move()
    {
        // 플레이어의 위치에 따라 스프라이트 방향 설정, 몬스터가 왼쪽에 있으면 flip
        spriteRenderer.flipX = transform.position.x < player.transform.position.x;
        if (isDeath == false) // 몬스터가 살아있을 때만
        {
            if (isAttack == false) // 몬스터가 공격중이 아닐 때
            {
                // 플레이어의 위치로 이동
                Vector3 direction = (player.transform.position - gameObject.transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);


            }
        }
    }
    void Attack()
    {
        float realDistance = Vector3.Distance(transform.position, player.transform.position);
        if (realDistance <= attackDistance)
        {
            isAttack = true;
            animator.SetTrigger("Attack");


        }
        else
        {
            isAttack = false;
        }
    }
    public void FireWithAnim()
    {
        Debug.Log("애니메이션 중 특정 시점에서 실행됨!");
        FireProjectiles2(); // 예: 투사체 발사
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.BossThrow);
    }

    // 원형 투사체 발사 메서드1
    private void FireProjectiles()
    {
        float angleStep = 360f / projectileCount;
        float angle = balanceAngle;


        for (int i = 0; i < projectileCount; i++)
        {
            // 투사체 방향 계산
            float projectileDirX = Mathf.Sin((angle * Mathf.PI) / 180) * projectileSpeed;
            float projectileDirY = Mathf.Cos((angle * Mathf.PI) / 180) * projectileSpeed;
            Vector3 projectileMoveDirection = new Vector3(projectileDirX, projectileDirY, 0).normalized * projectileSpeed;

            // 보스 위치에서 투사체 생성
            GameObject tempProjectile = Instantiate(projectileObject, transform.position, Quaternion.identity);

            //참조, 투사체에 보스의 공격력 부여
            BossProjectile_S5 Projectile = projectileObject.GetComponent<BossProjectile_S5>();
            Projectile.SetBoss(this);

            Rigidbody2D tempRb = tempProjectile.GetComponent<Rigidbody2D>();
            if (tempRb != null)
            {
                tempRb.linearVelocity = projectileMoveDirection; // 투사체 속도 설정
            }

            // 투사체의 회전 설정 (방향에 맞게)
            SpriteRenderer projectileSprite = tempProjectile.GetComponent<SpriteRenderer>();
            if (projectileSprite != null)
            {
                // 이동 방향에 따라 회전 계산
                float rotationAngle = Mathf.Atan2(projectileMoveDirection.y, projectileMoveDirection.x) * Mathf.Rad2Deg + 180f;
                tempProjectile.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
            }
            angle += angleStep;
        }
    }

    // 원형 투사체 발사 메서드2
    public void FireProjectiles2()
    {
        float angleStep = 360f / projectileCount;
        float angle = balanceAngle;

        for (int i = 0; i < projectileCount; i++)
        {
            // 투사체 방향 계산
            float projectileDirX = Mathf.Sin((angle * Mathf.PI) / 180) * projectileSpeed;
            float projectileDirY = Mathf.Cos((angle * Mathf.PI) / 180) * projectileSpeed;
            Vector3 projectileMoveDirection = new Vector3(projectileDirX, projectileDirY, 0).normalized * projectileSpeed2;
            GameObject tempProjectile = Instantiate(projectileObject2, launcher.transform.position, Quaternion.identity);
            
            //참조, 투사체에 보스의 공격력 부여
            BossProjectile_S5 Projectile = projectileObject2.GetComponent<BossProjectile_S5>();
            Projectile.SetBoss(this);
            
            Rigidbody2D tempRb = tempProjectile.GetComponent<Rigidbody2D>();
            
            if (tempRb != null)
            {
                tempRb.linearVelocity = projectileMoveDirection; // 투사체 속도 설정
            }
            
            // 투사체의 회전 설정 (방향에 맞게)
            SpriteRenderer projectileSprite = tempProjectile.GetComponent<SpriteRenderer>();
            if (projectileSprite != null)
            {
                // 이동 방향에 따라 회전 계산
                float rotationAngle = Mathf.Atan2(projectileMoveDirection.y, projectileMoveDirection.x) * Mathf.Rad2Deg;
                tempProjectile.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
            }
            angle += angleStep;
        }
    }
}
