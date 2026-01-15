using UnityEngine;

/// <summary>
/// Player에 스크립트 붙였음
/// </summary>

public class PlayerHp : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerHpUI _playerHpUI;
    [SerializeField] private PlayerHpBarUIShadow _hpBarShadow;

    private float _currentHp;
    private PlayerController _player;

    // MaxHp 변동 감지용 캐시
    private float _cachedMaxHp = -1.0f;

    public float CurrentHp => _currentHp;


    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentHp = _player.MaxHp;

        _cachedMaxHp = _player.MaxHp;

        if (_playerHpUI != null)
        {
            _playerHpUI.Init(_player.MaxHp);
        }

        UpdateHpUI();
    }

    public void TakeDamage(float damage)
    {
        if (_currentHp <= 0.0f)
            return;

        float finalDamage = Mathf.Max(1.0f, damage - _player.Defense);
        _currentHp -= finalDamage;
        _currentHp = Mathf.Clamp(_currentHp, 0.0f, _player.MaxHp);

        // 피격 순간 테두리 번쩍 효과
        if (_hpBarShadow != null)
        {
            _hpBarShadow.PlayHitFlash();
        }

        UpdateHpUI();

        if (_currentHp <= 0.0f)
        {
            Die();
        }

    }

    public void Heal(float amount)
    {
        if (amount <= 0.0f)
            return;

        _currentHp = Mathf.Min(_currentHp + amount, _player.MaxHp);
        UpdateHpUI();
    }

    private void UpdateHpUI()
    {
        // MaxHp가 바뀌었으면 UI 기준값 갱신
        float maxHp = _player.MaxHp;
        if (!Mathf.Approximately(_cachedMaxHp, maxHp))
        {
            float delta = maxHp - _cachedMaxHp;
            _cachedMaxHp = maxHp;

            if (delta > 0.0f)
            {
                _currentHp += delta;
            }

            // MaxHp가 늘어났는데 현재 체력이 최대를 넘지 않게 Clamp
            _currentHp = Mathf.Clamp(_currentHp, 0.0f, maxHp);

            if (_playerHpUI != null)
            {
                _playerHpUI.Init(maxHp);
            }
        }

        if (_playerHpUI == null && _hpBarShadow == null)
            return;

        float hpRatio = (maxHp > 0.0f) ? (_currentHp / maxHp) : 0.0f;

        if (_playerHpUI != null)
        {
            _playerHpUI.SetHp(_currentHp);
        }

        if (_hpBarShadow != null)
        {
            _hpBarShadow.SetDanger(hpRatio <= 0.2f);
        }
    }

    public void RefreshUI()
    {
        UpdateHpUI();
    }

    private void Die()
    {
        if (_hpBarShadow != null)
        {
            _hpBarShadow.SetDanger(false);
        }

        if (_player != null)
        {
            _player.StateMachine.ChangeState(_player.DeadState);
        }
    }
}
