using UnityEngine;

public class PlayerProjectile : Projectile
{
    /*Projectile에서 상속받음
    public float projectileSpeed = 8f; //투사체 속도
    public int damage = 1; //투사체 피해
    */
    public float knockbackPower;//투사체 넉백 계수
    public int pentration; //관통력
    public Vector3 direction;

    public void Initialize(Player player)
    {
        projectileSpeed = player.projectileSpeed;
        damage = player.atkPower;
        knockbackPower = player.knockbackPower;
        pentration = player.projectilePenetration;
    }
    private void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        rb.linearVelocity = direction * projectileSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster.isDeath) return;

            monster.TakeDamage(damage);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
            //넉백처리
            //보스는 넉백 안되게 만들어도 됨.
            //rigidbody 있으면 add force하면 될 것 같은데
            //transform 처리?

            pentration--;
            if (pentration <= 0)
            {
                PoolManager.Instance.Return(gameObject);  // 충돌 시 풀로 반환
            }
        }
    }
}
