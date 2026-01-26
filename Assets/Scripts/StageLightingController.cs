using System.Collections;
using UnityEngine;

/// <summary>
/// StageManager의 스테이지 변경 이벤트를 받아
/// Skybox / Directional Light / Ambient / Fog 등을 간단히 교체하는 컨트롤러
/// (조명/환경광은 1초 보간)
/// </summary>
public class StageLightingController : MonoBehaviour
{
    [SerializeField] private StageManager _stageManager;

    [Tooltip("태양 역할의 Directional Light를 연결하세요.")]
    [SerializeField] private Light _sunLight;

    [Header("Transition")]
    [Tooltip("스테이지 전환 보간 시간(초)")]
    [SerializeField] private float _transitionDuration = 1.0f;

    [Header("Skybox")]
    [Tooltip("Sunset 스테이지2에서 사용할 Skybox Material (없으면 기존 유지)")]
    [SerializeField] private Material _sunsetSkybox;

    [Tooltip("Night 스테이3에서 사용할 Skybox Material (없으면 기존 유지)")]
    [SerializeField] private Material _nightSkybox;

    [Tooltip("Dawn 스테이지4에서 사용할 Skybox Material (없으면 기존 유지)")]
    [SerializeField] private Material _dawnSkybox;

    [Tooltip("SnowDay 스테이지5에서 사용할 Skybox Material (없으면 기존 유지)")]
    [SerializeField] private Material _snowDaySkybox;

    [Header("Lighting Presets")]
    [Tooltip("Sunset(해질녘)")]
    [SerializeField] private Color _sunsetSunColor = new Color(1.0f, 0.55f, 0.25f, 1.0f);

    [Tooltip("Night(밤)")]
    [SerializeField] private Color _nightSunColor = new Color(0.55f, 0.65f, 1.0f, 1.0f);

    [Tooltip("Dawn(새벽)")]
    [SerializeField] private Color _dawnSunColor = new Color(0.95f, 0.75f, 0.55f, 1.0f);

    [Tooltip("SnowDay(낮, 눈내림)")]
    [SerializeField] private Color _snowDaySunColor = new Color(0.9f, 0.95f, 1.0f, 1.0f);

    [Space(8)]
    [Tooltip("Sunset 태양 강도(밝기)")]
    [SerializeField] private float _sunsetSunIntensity = 1.1f;

    [Tooltip("Night 태양 강도(밝기)")]
    [SerializeField] private float _nightSunIntensity = 0.25f;

    [Tooltip("Dawn 태양 강도(밝기)")]
    [SerializeField] private float _dawnSunIntensity = 0.6f;

    [Tooltip("SnowDay 태양 강도(밝기)")]
    [SerializeField] private float _snowDaySunIntensity = 1.0f;

    [Header("Ambient Presets")]
    [Tooltip("Sunset 환경광(전체 색감)")]
    [SerializeField] private Color _sunsetAmbient = new Color(0.35f, 0.25f, 0.2f, 1.0f);

    [Tooltip("Night 환경광(전체 어둡게)")]
    [SerializeField] private Color _nightAmbient = new Color(0.06f, 0.07f, 0.1f, 1.0f);

    [Tooltip("Dawn 환경광(밤인데 살짝 밝아지는 느낌)")]
    [SerializeField] private Color _dawnAmbient = new Color(0.18f, 0.18f, 0.22f, 1.0f);

    [Tooltip("SnowDay 환경광(차갑고 깨끗한 느낌)")]
    [SerializeField] private Color _snowDayAmbient = new Color(0.35f, 0.38f, 0.42f, 1.0f);

    // ===================== 캐시(원래 값 복구용) =====================
    private Material _defaultSkybox;
    private Color _defaultAmbient;

    private Color _defaultSunColor;
    private float _defaultSunIntensity;

    // 보간 코루틴 핸들(연속 전환 시 이전 전환 중단)
    private Coroutine _transitionCoroutine = null;

    private void Awake()
    {
        // 기본값 캐싱: Day1 (씬에 세팅된 값)
        _defaultSkybox = RenderSettings.skybox;
        _defaultAmbient = RenderSettings.ambientLight;

        if (_sunLight != null)
        {
            _defaultSunColor = _sunLight.color;
            _defaultSunIntensity = _sunLight.intensity;
        }
        else
        {
            Debug.LogWarning("[StageLightingController] Sun Light(Directional Light)가 연결되지 않았습니다.");
        }

        if (_stageManager == null)
        {
            Debug.LogWarning("[StageLightingController] StageManager가 연결되지 않았습니다.");
        }
    }

    private void OnEnable()
    {
        if (_stageManager != null)
            _stageManager.OnStageChanged += HandleStageChanged;
    }

    private void OnDisable()
    {
        if (_stageManager != null)
            _stageManager.OnStageChanged -= HandleStageChanged;
    }

    private void Start()
    {
        // 시작하자마자 현재 스테이지를 강제로 1회 적용
        // (웨이브 시작 이벤트를 기다리지 않아도 바로 라이팅/스카이박스가 맞춰짐)
        if (_stageManager != null)
            _stageManager.ForceBroadcastCurrentStage();
        else
            HandleStageChanged(StageType.Day1); // StageManager가 없으면 안전하게 Day1 기본 적용
    }

    private void HandleStageChanged(StageType stage)
    {
        // 목표 프리셋 결정
        Material targetSkybox = _defaultSkybox;
        Color targetSunColor = _defaultSunColor;
        float targetSunIntensity = _defaultSunIntensity;
        Color targetAmbient = _defaultAmbient;

        switch (stage)
        {
            case StageType.Day1:
                // Day1은 기본값 복구
                targetSkybox = _defaultSkybox;
                targetSunColor = _defaultSunColor;
                targetSunIntensity = _defaultSunIntensity;
                targetAmbient = _defaultAmbient;
                break;

            case StageType.Sunset:
                targetSkybox = _sunsetSkybox != null ? _sunsetSkybox : RenderSettings.skybox;
                targetSunColor = _sunsetSunColor;
                targetSunIntensity = _sunsetSunIntensity;
                targetAmbient = _sunsetAmbient;
                break;

            case StageType.Night:
                targetSkybox = _nightSkybox != null ? _nightSkybox : RenderSettings.skybox;
                targetSunColor = _nightSunColor;
                targetSunIntensity = _nightSunIntensity;
                targetAmbient = _nightAmbient;
                break;

            case StageType.Dawn:
                targetSkybox = _dawnSkybox != null ? _dawnSkybox : RenderSettings.skybox;
                targetSunColor = _dawnSunColor;
                targetSunIntensity = _dawnSunIntensity;
                targetAmbient = _dawnAmbient;
                break;

            case StageType.SnowDay:
                targetSkybox = _snowDaySkybox != null ? _snowDaySkybox : _defaultSkybox;
                targetSunColor = _snowDaySunColor;
                targetSunIntensity = _snowDaySunIntensity;
                targetAmbient = _snowDayAmbient;
                break;
        }

        // Skybox는 시작 시점에 즉시 교체 (Material 보간은 일반적으로 비추천)
        ApplySkybox(targetSkybox);

        // 기존 전환이 진행 중이면 중단 후 새 전환 시작
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }

        _transitionCoroutine = StartCoroutine(Co_LerpLighting(targetSunColor, targetSunIntensity, targetAmbient, _transitionDuration));
    }

    private IEnumerator Co_LerpLighting(Color targetSunColor, float targetSunIntensity, Color targetAmbient, float duration)
    {
        // duration이 0이면 즉시 적용
        if (duration <= 0.0001f)
        {
            ApplySun(targetSunColor, targetSunIntensity);
            ApplyAmbient(targetAmbient);
            _transitionCoroutine = null;
            yield break;
        }

        // 시작값(현재 상태)
        Color startSunColor = (_sunLight != null) ? _sunLight.color : Color.white;
        float startSunIntensity = (_sunLight != null) ? _sunLight.intensity : 1.0f;
        Color startAmbient = RenderSettings.ambientLight;

        float t = 0.0f;

        // 1초 동안 부드럽게(선형) 보간
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            Color sunColor = Color.Lerp(startSunColor, targetSunColor, p);
            float sunIntensity = Mathf.Lerp(startSunIntensity, targetSunIntensity, p);
            Color ambient = Color.Lerp(startAmbient, targetAmbient, p);

            ApplySun(sunColor, sunIntensity);
            ApplyAmbient(ambient);

            yield return null;
        }

        // 마지막 값 보정
        ApplySun(targetSunColor, targetSunIntensity);
        ApplyAmbient(targetAmbient);

        _transitionCoroutine = null;
    }

    private void ApplySkybox(Material mat)
    {
        // mat이 null이면 기존 유지
        if (mat == null)
            return;

        RenderSettings.skybox = mat;

        // Skybox 변경 후에는 GI 업데이트가 필요할 수 있음
        // (에디터/플레이에서 반영이 늦는 경우가 있어서 호출)
        DynamicGI.UpdateEnvironment();
    }

    private void ApplySun(Color color, float intensity)
    {
        if (_sunLight == null)
            return;

        _sunLight.color = color;
        _sunLight.intensity = intensity;
    }

    private void ApplyAmbient(Color ambientColor)
    {
        RenderSettings.ambientLight = ambientColor;
    }

}
