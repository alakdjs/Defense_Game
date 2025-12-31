using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _fullHpColor = Color.green;
    [SerializeField] private Color _middleHpColor = Color.yellow;
    [SerializeField] private Color _lowHpColor = Color.red;

    [SerializeField] private bool _followTarget = true;
    [SerializeField] private Vector3 _worldOffset = Vector3.up;

    private Transform _target;
    private Camera _mainCamera;

    private float _maxHp;
    private float _currentHp;

    // 초기화
    public void Init(Transform target, float maxHp)
    {
        _target = target;
        _maxHp = Mathf.Max(1.0f, maxHp);
        _currentHp = maxHp;

        _mainCamera = Camera.main;

        UpdateHpBar();
    }

    // World Offset 설정 (머리 위)
    public void SetWorldOffset(Vector3 offset)
    {
        _worldOffset = offset;
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

        // 플레이어 기준 거리 체크
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(_target.position, player.transform.position);

        }

        gameObject.SetActive(true);

        // 월드 좌표 -> 화면 좌표 변환
        Vector3 worldPos = _target.position + _worldOffset;
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        gameObject.SetActive(true);
        transform.position = screenPos;
    }
}
