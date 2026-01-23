using System.Collections.Generic;
using UnityEngine;

public class TowerMain : MonoBehaviour, IDamageable
{
    [Header("Tower Stat")]
    [SerializeField] private float _maxHp = 1000.0f;
    [SerializeField] private float _defense = 1.0f;
    [SerializeField] private float _petRadius = 15.0f; // 펫(서브타워) 이동 반경

    [Header("UI")]
    [SerializeField] private PlayerHpUI _towerHpUI;
    [SerializeField] private PlayerHpBarUIShadow _hpBarShadow;
    private float _currentHp;

    [Header("Pet Spwan")]
    [SerializeField] private List<Transform> _petSpawnPoints = new List<Transform>();
    private readonly List<PetBase> _spawnedPets = new List<PetBase>();
    
    public IReadOnlyList<PetBase> SpawnedPets => _spawnedPets;
    public float PetRadius => _petRadius;

    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;

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

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        TakeDamage(damageInfo.damage);
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

    private void OnDrawGizmos() // 펫 이동반경 표시용도
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position, _petRadius);
    }

    public void Heal(float amount)
    {
        if (amount <= 0.0f)
            return;

        if (_currentHp <= 0.0f)
            return;

        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);

        if (_towerHpUI != null)
        {
            _towerHpUI.SetHp(_currentHp);
        }

        UpdateHpUI();
    }

    /// <summary>
    /// 타워 최대 체력 증가 (증강)
    /// </summary>
    public void AddMaxHp(float addValue)
    {
        _maxHp += addValue;

        _currentHp += addValue;
        _currentHp = Mathf.Clamp(_currentHp, 0.0f, _maxHp);

        if (_towerHpUI != null)
        {
            _towerHpUI.Init(_maxHp);
            _towerHpUI.SetHp(_currentHp);
        }

        UpdateHpUI();

        Debug.Log($"[타워 증강] 최대 체력 + {addValue} -> {_maxHp}");
    }

    /// <summary>
    /// 타워 방어력 증가 (증강)
    /// </summary>
    public void AddDefense(float addValue)
    {
        _defense += addValue;
        Debug.Log($"[타워 증강] 방어력 + {addValue} -> {_defense}");
    }

    /// <summary>
    /// 펫 소환 (증강)
    /// </summary>
    public PetBase SpawnPet(GameObject petPrefab)
    {
        Vector3 spawnPos = _petSpawnPoints[0].position;

        GameObject go = Instantiate(petPrefab, spawnPos, Quaternion.identity);

        PetBase pet = go.GetComponent<PetBase>();
        pet.SetTower(transform);

        _spawnedPets.Add(pet);

        Debug.Log($"[TowerMain] 펫 소환: {petPrefab.name} (현재 {_spawnedPets.Count}마리)");
        return pet;
    }
}
