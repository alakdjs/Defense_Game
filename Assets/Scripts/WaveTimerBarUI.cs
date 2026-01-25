using UnityEngine;
using UnityEngine.UI;


public class WaveTimerBarUI : MonoBehaviour
{
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _waveNameText;
    [SerializeField] private Text _bossHpText;

    private Color _defaultFillColor = Color.white;

    private void Awake()
    {
        if (_fillImage == null)
            Debug.LogWarning("[WaveTimerBarUI] Fill Image가 연결되지 않았습니다.");
        else
            _defaultFillColor = _fillImage.color;
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

        _bossHpText.gameObject.SetActive(false);

        // 일반 웨이브: 남은 시간 표시, 진행률(0~1)
        float p = _waveManager.CurrentWaveProgress01;

        if (_fillImage != null)
        {
            _fillImage.fillAmount = 1.0f - p;
            _fillImage.color = _defaultFillColor;
        }

        if (_timeText != null)
        {
            // 남은 시간 mm:ss
            float remain = _waveManager.CurrentWaveRemainingTime;
            int totalSec = Mathf.CeilToInt(remain);
            int m = totalSec / 60;
            int s = totalSec % 60;
            _timeText.text = $"{m:00}:{s:00}";
        }

        // 보스 웨이브: 보스 HP 표시
        if (_waveManager.IsBossWave && _waveManager.HasBossSpawned &&_waveManager.BossInstance != null)
        {
            _fillImage.fillAmount = _waveManager.BossInstance.HpRatio01;
            _fillImage.color = Color.red;

            if (_waveNameText != null)
                _waveNameText.text = $"{_waveManager.CurrentWaveName} BOSS !";

            if (_timeText != null)
            {
                _bossHpText.text = $"{_waveManager.BossInstance.CurrentHp} / {_waveManager.BossInstance.MaxHp}";
                _bossHpText.gameObject.SetActive(true);
                _timeText.text = "";
            }

            return;
        }

    }

}
