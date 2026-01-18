using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
    // HpBar 자동 숨김 타이머
    [SerializeField] private bool _useAutoHide = false;
    [SerializeField] private float _autoHideDelay = 1.5f;
    private float _hideTimer = 0.0f;
    private bool _autoHide = false;

    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _fullHpColor = Color.green;
    [SerializeField] private Color _middleHpColor = Color.yellow;
    [SerializeField] private Color _lowHpColor = Color.red;

    [SerializeField] private bool _followTarget = true;
    [SerializeField] private Vector3 _worldOffset = Vector3.up;

    [SerializeField] private bool _monsterHp = false;

    private Transform _target;
    private Camera _mainCamera;

    private float _maxHp;
    private float _currentHp;

    // 초기화
    public void Init(Transform target, float maxHp, bool monsterHp, bool useAutoHide)
    {
        _target = target;
        _monsterHp = monsterHp;
        _useAutoHide = useAutoHide;

        _maxHp = Mathf.Max(1.0f, maxHp);
        _currentHp = maxHp;

        _mainCamera = Camera.main;

        if (!_useAutoHide)
            gameObject.SetActive(true);

        UpdateHpBar();
    }

    public void ShowHpBar()
    {
        if (!_useAutoHide)
            return;

        _autoHide = true;
        _hideTimer = _autoHideDelay;
        gameObject.SetActive(true);
    }

    // World Offset 설정 (머리 위)
    public void SetWorldOffset(Vector3 offset)
    {
        _worldOffset = offset;
    }

    // 최대 체력 갱신(펫 강화)
    public void SetMaxHp(float maxHp)
    {
        float oldMax = _maxHp;
        _maxHp = Mathf.Max(1.0f, maxHp);

        if (oldMax > 0.0f)
        {
            float ratio = _currentHp / oldMax;
            _currentHp = _maxHp * ratio;
        }

        _currentHp = Mathf.Clamp(_currentHp, 0.0f, _maxHp);
        UpdateHpBar();
    }

    // 체력 갱신
    public void SetHp(float currentHp)
    {
        _currentHp = Mathf.Clamp(currentHp, 0.0f, _maxHp);
        UpdateHpBar();
    }

    // 체력바 UI 갱신
    private void UpdateHpBar()
    {
        if (_fillImage == null)
            return;

        float ratio = _currentHp / _maxHp;
        _fillImage.fillAmount = ratio; // 체력 비율에 따른 FillAmount

        if (_monsterHp)
        {
            _fillImage.color = Color.red;
            return;
        }

        if (ratio >= 0.66f)
        {
            _fillImage.color = _fullHpColor;
        }
        else if (ratio >= 0.33f)
        {
            _fillImage.color = _middleHpColor;
        }
        else
        {
            _fillImage.color = _lowHpColor;
        }
    }

    private void LateUpdate()
    {
        // 따라다니지 않는 UI (플레이어 UI 등) 위치 갱신 안함
        if (_followTarget == false)
            return;

        // 타겟이 사라졌으면 체력바도 제거
        if (_target == null || _mainCamera == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_autoHide)
        {
            _hideTimer -= Time.deltaTime;
            if (_hideTimer <= 0.0f)
            {
                gameObject.SetActive(false);
                _autoHide = false;
                return;
            }
        }

        // 월드 좌표 -> 화면 좌표 변환
        Vector3 worldPos = _target.position + _worldOffset;
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
    }
}
