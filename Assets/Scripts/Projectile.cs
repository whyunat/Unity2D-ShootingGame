using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float projectileSpeed = 8f; //투사체 속도
    public int damage = 1; //투사체 피해

    [SerializeField] protected Rigidbody2D rb;
    //투사체 이동 메서드
    protected abstract void Move();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            PoolManager.Instance.Return(gameObject);
        }
    }

    protected void OnBecameInvisible()
    {
        PoolManager.Instance.Return(gameObject);
    }
}
