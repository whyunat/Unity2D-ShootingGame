using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Shoom : Monster
{
    /*
    [Header("몬스터 속성")]
    public float speed = 1.5f;
    public GameObject[] expOrb;
    public float attackDistance = 5f;

    public GameObject player;
    protected bool isDeath = false;
    protected bool isAttack = false;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    */

    public float findDistance = 3f;
    public float velPower = 1.5f;

    private bool isAttacking;
    private float atkDelay;

    private Rigidbody2D rb;
    private NavMeshAgent navMeshAgent;
    protected override void Death()
    {
        animator.SetBool("isDeath", true);

        if (!isDeath)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ShoomDie);
        }
        isDeath = true;
        StartCoroutine(ReturnToPoolAfterDelay(0.7f)); // 0.7초후 풀반환
        CreateExpOrb();
        GameManager.Instance.KillScore++;
    }


    protected override void Move()
    {
        if (isDeath == false) // 몬스터가 살아있을 때만
        {
            if (isAttack == false) // 몬스터가 공격중이 아닐 때
            {
                // 플레이어의 위치로 이동
                //Vector3 direction = (player.transform.position - gameObject.transform.position).normalized;
                //transform.Translate(direction * moveSpeed * Time.deltaTime);
                navMeshAgent.SetDestination(player.transform.position);
            }
        }
    }

    // delay 시간 이후 오브젝트 풀에 리턴하는 코루틴
    private IEnumerator ReturnToPoolAfterDelay(float delay)
    { 
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.Return(gameObject);
    }

    private void SetUp()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        if (navMeshAgent == null)
        {
            Debug.LogError("네비없음");
        }
        if (player == null)
        {
            Debug.LogError("플레이어 없음");
        }
    }

    void Start()
    {
        SetUp();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        AtkDelay(); // 초기 delay 계산
    }

    void Update()
    {
        Flip();
        Move();
        Find();
        Attack();
    }

    private void AtkDelay()
    {
        atkDelay = 10f / atkSpeed;
    }

    private void Flip()
    {
        // 플레이어의 위치에 따라 스프라이트 방향 설정, 몬스터가 오른쪽에 있으면 flip
        spriteRenderer.flipX = transform.position.x > player.transform.position.x;
    }
    private void Find()
    {
        float realDistance = Vector3.Distance(transform.position, player.transform.position);
        if (realDistance <= findDistance)
        {
            animator.SetBool("isFound", true);
        }
        else
        {
            animator.SetBool("isFound", false);
        }

    }

    void Attack()
    {
        float realDistance = Vector3.Distance(transform.position, player.transform.position);
        if (realDistance <= attackDistance && isAttacking == false)
        {
          
            // 플레이어 방향 계산
            Vector3 direction = (player.transform.position - transform.position).normalized;

            // 힘 적용 (예: 500f는 힘의 크기, 필요에 따라 조정)
            rb.linearVelocity = direction * velPower;

            animator.SetTrigger("Attack");


            StartCoroutine(AttackCooldown(atkDelay));
        }
        else
        {
            
        }
    }
    IEnumerator AttackCooldown(float delay)
    {
        isAttacking = true; // 공격 중 플래그 설정

        yield return new WaitForSeconds(delay); // 공격 속도만큼 대기

        rb.linearVelocity = Vector2.zero; // 공격 후 속도 초기화
        isAttacking = false; // 공격 가능 상태로 전환
    }
}
