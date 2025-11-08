using UnityEngine;

public class BossProjectile_S5 : Projectile
{

    private Boss_S5 boss;

    public void SetBoss(Boss_S5 bossRef)
    {
        boss = bossRef; // 보스 정보 저장
        damage = boss.atkPower; // 보스의 공격력을 가져와서 적용
    }
    public Vector3 direction;

    

    /*
    public float projectileSpeed = 8f; //투사체 속도
    public int damage = 1; //투사체 피해
     */

    protected override void Move()
    {
        rb.linearVelocity = direction * projectileSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
      
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player"))
        {
            PoolManager.Instance.Return(gameObject);

            collision.GetComponent<Player>().TakeDamage(damage);
        }

        
    }
}