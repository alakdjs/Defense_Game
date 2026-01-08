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

    public float CurrentHp => _currentHp;


    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentHp = _player.MaxHp;

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
        if (_playerHpUI == null && _hpBarShadow == null)
            return;

        float maxHp = _player.MaxHp;
        float hpRatio = _currentHp / maxHp;

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
