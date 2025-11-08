using System.Collections;
using UnityEngine;

public class Spider_Missile : MonoBehaviour
{
    public float speed = 5f;    // 미사일 속도
    public float lifeTime = 2f; // 미사일 생존 시간
    public int damage = 0;      // 미사일 데미지 0으로 초기화
    public float webDuration = 2f; // 거미줄 지속시간

    private Vector2 direction;  // 미사일 이동 방향
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool hasCollided = false; // 충돌 여부 확인

    public float speedReductionFactor = 0.5f;
    private Player player;

    [Header("거미줄 마지막 프레임 스프라이트")]
    public Sprite lastWebSprite;   // 가장 큰 거미줄 이미지

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifeTime);  // 일정 시간 후 미사일 제거     
    }

    public void SetDamage(int monsterDamage)
    {
        damage = monsterDamage;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasCollided)
            {
                player = collision.GetComponent<Player>();
                if (player != null)
                {
                    player.ApplySlowEffect(speedReductionFactor);
                }
                hasCollided = true;
                speed = 0;

                collision.GetComponent<Player>().TakeDamage(damage);

                // 마지막 프레임 유지
                StartCoroutine(HoldLastFrame());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player != null)
        {
            player.RemoveSlowEffect(speedReductionFactor);
        }
    }

    private IEnumerator HoldLastFrame()
    {
        // 애니메이션 멈추고 마지막 거미줄 이미지로 변경
        animator.enabled = false;
        if (lastWebSprite != null)
        {
            spriteRenderer.sprite = lastWebSprite;
        }

        // 일정 시간 후 오브젝트 제거
        yield return new WaitForSeconds(webDuration);
        Destroy(gameObject);
    }
}
