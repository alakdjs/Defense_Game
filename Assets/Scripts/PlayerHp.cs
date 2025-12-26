using UnityEngine;

/// <summary>
/// Player에 스크립트 붙였음
/// </summary>

public class PlayerHp : MonoBehaviour
{
    [SerializeField] private PlayerHpUI _playerHpUI;
    [SerializeField] private PlayerHpBarUIShadow _hpBarShadow;

    [SerializeField] private float _maxHp = 100.0f;
    private float _currentHp;

    private PlayerController _player;
    private HpBar _hpBar;

    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentHp = _maxHp;

        if (_playerHpUI != null)
        {
            _playerHpUI.Init(_maxHp);
        }

        UpdateHpUI();
    }

    public void TakeDamage(float damage)
    {
        if (_currentHp <= 0.0f)
            return;

        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0.0f, _maxHp);

        // 피격 순간 테두리 번쩍 효과
        if (_hpBarShadow != null )
        {
            _hpBarShadow.PlayHitFlash();
        }

        UpdateHpUI();

        if (_currentHp <= 0.0f)
        {
            Die();
        }

    }

    private void UpdateHpUI()
    {
        float hpRatio = _currentHp / _maxHp;

        if (_playerHpUI != null)
        {
            _playerHpUI.SetHp(_currentHp);
        }

        if (_hpBarShadow  != null)
        {
            _hpBarShadow.SetDanger(hpRatio <= 0.2f);
        }
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
