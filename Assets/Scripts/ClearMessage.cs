using System.Collections;
using TMPro;
using UnityEngine;

public class ClearMessage : MonoBehaviour
{
    //색상 전환에 걸리는 시간
    [SerializeField]
    float lerpTime = 0.2f;

    //텍스트 컴포넌트
    TextMeshProUGUI Clear;


    //Awake 메서드 : 컴포넌트 초기화
    private void Awake()
    {
        Clear = GetComponent<TextMeshProUGUI>();
    }

    //OnEnable메서드 : 오브젝트가 활성화될때 호출
    private void OnEnable()
    {
        Debug.Log("ClearMessage 활성화됨!"); // 디버깅 로그 추가
        StartCoroutine("ColorLerpLoop");
    }

    //색상 전환 루프 코루틴
    IEnumerator ColorLerpLoop()
    {
        while (true)
        {
            yield return StartCoroutine(ColorLerp(Color.white, Color.green));
            yield return StartCoroutine(ColorLerp(Color.green, Color.yellow));
            yield return StartCoroutine(ColorLerp(Color.yellow, Color.white));
        }
    }

    //색상 전환 코루틴
    IEnumerator ColorLerp(Color startColor, Color endColor)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;
            Clear.color = Color.Lerp(startColor, endColor, percent);
            yield return null;
        }
    }
}