using UnityEngine;


public class SnowUpgradeController : MonoBehaviour
{
    [SerializeField] private StageManager _stageManager;

    [Tooltip("기본 눈(Soft). 항상 파티클 On")]
    [SerializeField] private ParticleSystem _softSnow;

    [Tooltip("폭설 눈(Heavy). SnowDay에서만 파티클 On")]
    [SerializeField] private ParticleSystem _heavySnow;

    private void Awake()
    {
        // Soft는 항상 켜둠
        if (_softSnow != null && _softSnow.isPlaying == false)
            _softSnow.Play();

        // Heavy는 기본적으로 꺼둠
        if (_heavySnow != null && _heavySnow.isPlaying)
            _heavySnow.Stop();
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
        // 시작 시 현재 스테이지 상태를 1회 반영
        if (_stageManager != null)
            _stageManager.ForceBroadcastCurrentStage();
        else
            HandleStageChanged(StageType.Day1);
    }

    private void HandleStageChanged(StageType stage)
    {
        // SnowDay에서만 Heavy를 켜서 폭설 업그레이드
        bool isSnowDay = (stage == StageType.SnowDay);

        if (_heavySnow == null)
            return;

        if (isSnowDay)
        {
            if (_heavySnow.isPlaying == false)
                _heavySnow.Play();
        }
        else
        {
            if (_heavySnow.isPlaying)
                _heavySnow.Stop();
        }
    }
}
