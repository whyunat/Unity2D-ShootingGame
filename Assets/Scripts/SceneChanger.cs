using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("충돌 감지! 전환할 씬 이름: Stage3");
            SceneManager.LoadScene("Stage3");
        }
    }
}
