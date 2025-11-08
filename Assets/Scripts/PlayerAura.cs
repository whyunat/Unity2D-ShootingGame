using UnityEngine;

public class PlayerAura : MonoBehaviour
{
    [SerializeField] private int damage;

    //추가
    private float damageInterval = 1f; // 1초 간격
    private float lastDamageTime; // 마지막 데미지 적용 시간

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            // 현재 시간이 마지막 데미지 시간 + 간격을 넘었는지 확인
            if (Time.time >= lastDamageTime + damageInterval)
            {
                collision.GetComponent<Monster>().TakeDamage(damage);
                lastDamageTime = Time.time; // 마지막 데미지 시간 업데이트
            }
        }
    }
}