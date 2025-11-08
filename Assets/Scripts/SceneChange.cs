using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = "Stage1"; // 기본값 설정

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("충돌 감지! 전환할 씬 이름: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}