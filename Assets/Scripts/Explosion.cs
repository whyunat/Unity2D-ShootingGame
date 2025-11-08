using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]private int damage;
    private void Awake()
    {
        Destroy(gameObject, 0.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(damage);
        }
    }
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
