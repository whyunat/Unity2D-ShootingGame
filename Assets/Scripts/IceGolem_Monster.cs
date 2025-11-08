using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IceGolem_Monster : Monster
{
    public float attackCooldown = 2f;
    private float attackTimer;
    private bool isAttacking = false; // 공격 중인지 확인
    public NavMeshAgent navMeshAgent = null;

    // delay 시간 이후 오브젝트 풀에 리턴하는 코루틴
    //private IEnumerator ReturnToPoolAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    PoolManager.Instance.Return(gameObject);
    //}

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackTimer = 0f;
        SetUp();
    }
    private void SetUp()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        if(navMeshAgent == null)
        {
            Debug.LogError("네비없음");
        }
        if (player == null) {
            Debug.LogError("플레이어 없음");
        }
    }

    void Update()
    {
        //base.Move();
        navMeshAgent.SetDestination(player.transform.position); 
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // 공격 쿨타임 감소
        }
    }

    // 충돌처리, 추후 플레이어 공격
    void OnTriggerEnter2D(Collider2D collider)
    {
        // 충돌한 객체가 플레이어라면
        if (collider.gameObject.CompareTag("Player") && attackTimer <= 0 && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }
    private IEnumerator AttackSequence()
    {
        animator.SetBool("isWalking", false);
        isAttacking = true; // 공격 시작
        attackTimer = attackCooldown; // 쿨타임 설정

        animator.SetTrigger("Attack");
        Debug.Log("너를 공격한다!");

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 공격이 끝나면 다시 걷는 애니메이션으로 변경
        isAttacking = false;
        animator.SetBool("isWalking", true);
    }

    protected override void Death()
    {
        animator.SetBool("isDeath", true);

        base.Death();
    }
}
