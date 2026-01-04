using UnityEngine;

public class TowerMain : MonoBehaviour, IDamageable
{
    [Header("Tower Stat")]
    [SerializeField] private float _maxHp = 100.0f;
    [SerializeField] private float _defense = 1.0f;
    [SerializeField] private float _petRadius = 5.0f; // 펫(서브타워) 이동 반경

    [Header("UI")]
    [SerializeField] private PlayerHpUI _towerHpUI;
    [SerializeField] private PlayerHpBarUIShadow _hpBarShadow;
    private float _currentHp;

    public float PetRadius => _petRadius;

    private void Awake()
    {
        _currentHp = _maxHp;

        if (_towerHpUI != null)
        {
            _towerHpUI.Init(_maxHp);
        }

        UpdateHpUI();
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage / _defense;
        finalDamage = Mathf.Max(1.0f, finalDamage);

        _currentHp -= finalDamage;
        _currentHp = Mathf.Clamp(_currentHp, 0.0f, _maxHp);

        if (_towerHpUI != null)
        {
            _towerHpUI.SetHp(_currentHp);
        }

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

    private void UpdateHpUI()
    {
        float maxHp = _maxHp;
        float hpRatio = _currentHp / maxHp;

        if (_towerHpUI != null)
        {
            _towerHpUI.SetHp(_currentHp);
        }

        if (_hpBarShadow != null)
        {
            _hpBarShadow.SetDanger(hpRatio <= 0.2f);
        }
    }

    private void Die()
    {
        // Game Over
    }
}
