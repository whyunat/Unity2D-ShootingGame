using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool isPlayerInPortal = false;

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PortalManager.Instance가 null입니다! 싱글턴이 설정되지 않았습니다.");
        }
        else if (isPlayerInPortal && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.LoadNextStage(); // 씬 전환 요청
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            Debug.Log("F 키를 눌러 이동하세요.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
            Debug.Log("포털 밖으로 나갔습니다.");
        }
    }
}
