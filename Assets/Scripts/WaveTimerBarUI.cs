using UnityEngine;
using UnityEngine.UI;


public class WaveTimerBarUI : MonoBehaviour
{
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _waveNameText;

    private void Awake()
    {
        if (_fillImage == null)
            Debug.LogWarning("[WaveTimerBarUI] Fill Image가 연결되지 않았습니다.");
    }

    private void OnEnable()
    {
        if (_waveManager != null)
            _waveManager.OnWaveStarted += RefreshWaveStaticTexts;
    }

    private void OnDisable()
    {
        if (_waveManager != null)
            _waveManager.OnWaveStarted -= RefreshWaveStaticTexts;
    }

    private void Start()
    {
        RefreshWaveStaticTexts();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void RefreshWaveStaticTexts()
    {
        if (_waveManager == null)
            return;

        // 웨이브 이름 같은 정적 정보는 웨이브 시작 시 갱신
        if (_waveNameText != null)
            _waveNameText.text = _waveManager.CurrentWaveName;
    }

    private void UpdateUI()
    {
        if (_waveManager == null)
            return;

        // 진행률(0~1)
        float p = _waveManager.CurrentWaveProgress01;

        if (_fillImage != null)
            _fillImage.fillAmount = 1.0f - p;

        if (_timeText != null)
        {
            // 남은 시간 mm:ss
            float remain = _waveManager.CurrentWaveRemainingTime;
            int totalSec = Mathf.CeilToInt(remain);
            int m = totalSec / 60;
            int s = totalSec % 60;
            _timeText.text = $"{m:00}:{s:00}";
        }
    }

}
