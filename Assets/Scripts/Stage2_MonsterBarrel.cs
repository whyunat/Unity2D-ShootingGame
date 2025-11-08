using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Stage2_MonsterBarrel : Monster
{
    /*
    [Header("몬스터 기본 속성")]
    public float moveSpeed = 5f; //이동 속도
    public int health = 2; //체력
    public int atkPower = 2; //공격력
    public float atkSpeed = 1f; //공격 속도
    [Header("몬스터 속성")]
    public float speed = 1.5f;
    public GameObject[] expOrb;
    public float attackDistance = 5f;
    */

    [Header("폭발 설정")]
    public float explosionRadius = 4.5f; // 폭발 반경
    public float detonationDistance = 2.0f; // 플레이어와 이 거리 내에 들어오면 폭발

    private float distanceToPlayer;
    private bool hasExploded = false;

    private NavMeshAgent navMeshAgent = null;
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

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetUp();
    }
    private void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detonationDistance && !hasExploded)
        {
            Explode();
        }
        else if(distanceToPlayer > detonationDistance)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("isMove", true);
        }
    }

    private void Explode()
    {
        hasExploded = true;
        animator.SetBool("isMove", false);
        animator.SetBool("hasExploded", hasExploded);
    }

    public void DestroyBarrel()//애니메이션 이벤트로 호출
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<Player>().TakeDamage(atkPower);
            }
        }
        base.Death();
    }

    private void OnDrawGizmosSelected()
    {
        // 폭발 범위 시각화 (디버깅 용도)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    protected override void Death()
    {
        base.Death();
    }
}
