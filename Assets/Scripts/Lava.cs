using UnityEngine;

public class Lava : MonoBehaviour
{
    private float collisionTime = 0f; // 충돌 지속 시간
    private const float damageDelay = 1f; // 1초 이상 충돌 시 동작 트리거
    private bool hasSlowedDown = false;
    public float speedReductionFactor = 0.5f;
    public int damage = 1;

    private float originalSpeed; // 플레이어의 원래 속도 저장
    private Player player; // 플레이어 참조 캐싱

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
           
            if(!hasSlowedDown)
            {
                player = collision.GetComponent<Player>();
                originalSpeed = player.moveSpeed; // 원래 속도 저장
                player.moveSpeed *= speedReductionFactor; // 속도 감소
                hasSlowedDown = true;
            }
            


           
            collisionTime += Time.deltaTime; // 충돌 시간 누적


            // 1초 이상 충돌 시 동작 실행
            if (collisionTime >= damageDelay)
            {
                TriggerLavaEffect();
                collisionTime = 0f; // 시간 초기화 (반복 실행 방지 또는 필요에 따라 조정)
                collision.GetComponent<Player>().TakeDamage(damage);

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어와 충돌이 끝나면 초기화
        if (collision.gameObject.CompareTag("Player"))
        {
           
            collisionTime = 0f;
            player.moveSpeed = originalSpeed;
            hasSlowedDown = false;
        }
    }
    

    private void TriggerLavaEffect()
    {
        // 여기서 주석 처리된 로직을 실행하거나 대체 동작을 정의
        Debug.Log("플레이어가 Lava와 1초 이상 충돌했습니다!");

        
    }
}
