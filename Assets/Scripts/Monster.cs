using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public class Monster : Character
{
    [Header("몬스터 속성")]
    public GameObject[] expOrb;
    public float attackDistance = 5f;

    protected GameObject player;
    public bool isDeath = false;
    protected bool isAttack = false;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    private int maxHealth;

    public GameObject heal;

    public void Initiate()
    {
        isDeath = false;
        health = maxHealth;
        Debug.Log($"{name} health: {health}");
    }

    private void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Awake()
    {
        maxHealth = health;
    }

    void Update()
    {
        Move();
    }

    protected override void Move()
    {
        // 플레이어의 위치로 이동
        Vector3 direction = (player.transform.position - gameObject.transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !isDeath)
        {
            Death();
        }
    }

    protected override void Death()
    {
        isDeath = true;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ShoomDie);
        GameManager.Instance.KillScore++;
        PoolManager.Instance.Return(gameObject);
        CreateExpOrb();
        //CreateHeal();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDeath && !isAttack && collision.CompareTag("Player"))
        {
            StartCoroutine(AttackRoutine(collision.GetComponent<Player>()));
        }
    }

    IEnumerator AttackRoutine(Player player)
    {
        isAttack = true;
        player.TakeDamage(atkPower);
        yield return new WaitForSeconds(2.5f);
        isAttack = false;
    }

    // 확률 설정한 랜덤 경험치
    protected void CreateExpOrb()
    {
        if (expOrb.Length == 0) return; // expOrb 배열이 비어 있으면 실행하지 않음

        // 0.0 ~ 1.0 사이의 랜덤 값 생성
        float randomValue = Random.value;

        // 확률에 따라 인덱스 선택
        int selectedIndex;
        if (randomValue < 0.5f)
        {
            selectedIndex = 0; // 50% 확률
        }
        else if (randomValue < 0.85f) // 0.5 + 0.35
        {
            selectedIndex = 1; // 35% 확률
        }
        else
        {
            selectedIndex = 2; // 15% 확률
        }

        // 선택된 인덱스로 오브젝트 생성
        GameObject expOrbInstance = Instantiate(expOrb[selectedIndex], gameObject.transform.position, Quaternion.identity);
        Destroy(expOrbInstance, 30f); // 15초 후 제거
    }

    //protected void CreateHeal()
    //{
    //    // 10% 확률로 heal 오브젝트 생성
    //    if (Random.value > 0.1f)
    //    {
    //        GameObject healInstance = Instantiate(heal, gameObject.transform.position, Quaternion.identity);
    //        Destroy(healInstance, 10f); // 10초 후 제거
    //    }
    //}
}
