using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("케릭터 기본 속성")]
    public float moveSpeed = 5f; //이동 속도
    public int health = 2; //체력
    public int atkPower = 2; //공격력
    public float atkSpeed = 1f; //공격 속도

    //이동 메서드
    protected abstract void Move();

    //피격 메서드
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    //사망 메서드
    protected abstract void Death();
}
