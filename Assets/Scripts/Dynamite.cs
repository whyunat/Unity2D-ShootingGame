using System.Collections;
using UnityEngine;

public class Dynamite : Projectile
{
    /*
    public float projectileSpeed = 8f; //투사체 속도
    public int damage = 1; //투사체 피해

    protected Rigidbody2D rb;
    */
    public GameObject explosion;
    public float lifeTime;
    private Vector2 direction;

    public void Initiate(float distance, int damage, Vector2 direction)
    {
        lifeTime = distance/projectileSpeed;// 거리/속력 == 시간
        this.damage = damage;
        this.direction = direction;
    }

    void Start()
    {
        StartCoroutine(ExplosionRoutine());
    }
    private void FixedUpdate()
    {
        Move();
    }

    IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);
        go.GetComponent<Explosion>().SetDamage(damage);
        PoolManager.Instance.Return(gameObject);
    }
    protected override void Move()
    {
        rb.linearVelocity = direction * projectileSpeed;
    }
}
