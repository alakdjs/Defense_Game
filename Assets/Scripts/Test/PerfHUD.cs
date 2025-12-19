using UnityEngine;

/// <summary>
/// 0.5초마다 평균 프레임/프레임타임 로그로 뿌려서
/// 60에 붙는지, 30에 붙는지, 스파이크가 있는지 확인.
/// </summary>
public class PerfHUD : MonoBehaviour
{
    float acc; int frames; float timer;
    void Update()
    {
        acc += Time.unscaledDeltaTime; frames++; timer += Time.unscaledDeltaTime;
        if (timer >= 0.5f)
        {
            float avg = acc / frames;
            int fps = Mathf.RoundToInt(1f / avg);
            Debug.Log($"[Perf] FPS={fps}, frameTime={(avg * 1000f):F1}ms");
            acc = 0f; frames = 0; timer = 0f;
        }
    }
}