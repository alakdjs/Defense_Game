using UnityEngine;

/// <summary>
/// 앱 시작 시 프레임 타깃을 명시하고(vSync 무시), 모바일에서 60fps를 목표로 잡는다.
/// </summary>
public class AppPerfBoot : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;   // 모바일에선 대부분 무시되지만 확실히 0으로
        Application.targetFrameRate = 60; // 60 고정(기기 여건되면 90/120도 가능)
    }
}
