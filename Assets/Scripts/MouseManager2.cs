using UnityEngine;


public class MouseManager2 : MonoBehaviour
{
    [SerializeField] private Transform player;     // 플레이어 위치 참조
    [SerializeField] private Camera mainCamera;    // 메인 카메라 참조
    [SerializeField] private float radius = 5f;    // 원의 반지름 (Inspector에서 조정 가능)

    private const float DEPTH_OFFSET = 10f;        // 카메라와 오브젝트 간 기본 거리

    public static MouseManager2 Instance { get; private set; }

    private void Awake()
    {
        SetupSingleton();
        EnsureReferences();
    }

    private void Update()
    {
        if (mainCamera == null || player == null) return;

        Vector3 targetPos = CalculateTargetPosition();
        transform.position = targetPos;
    }

    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureReferences()
    {
        // 카메라 자동 할당
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("메인 카메라를 찾을 수 없습니다.");
            }
        }

        // 플레이어 참조 확인
        if (player == null)
        {
            Debug.LogError("플레이어 Transform이 할당되지 않았습니다.");
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane + DEPTH_OFFSET)
        );

        // 플레이어 위치를 기준으로 마우스 방향 벡터 계산
        Vector3 direction = mousePos - player.position;

        // 플레이어로부터의 거리 계산
        float distance = direction.magnitude;

        // 거리가 반지름(radius) 이내이면 마우스 위치를 그대로 사용
        if (distance <= radius)
        {
            return new Vector3(mousePos.x, mousePos.y, 0);
        }
        // 반지름을 초과하면 방향 벡터를 정규화하고 원의 경계에 위치
        else
        {
            Vector3 clampedPos = player.position + direction.normalized * radius;
            return new Vector3(clampedPos.x, clampedPos.y, 0);
        }
    }

    public Quaternion GetRotationInfo()
    {
        return Quaternion.AngleAxis(90, Vector3.forward);
    }
}