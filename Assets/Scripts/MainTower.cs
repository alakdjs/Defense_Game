using UnityEngine;

public class MainTower : MonoBehaviour
{
    [Header("Tower Stat")]
    [SerializeField] private float _TowerHp = 100.0f; // 체력
    [SerializeField] private float _TowerDefense = 5.0f; // 방어력

    [SerializeField] private float _petRadius = 5.0f; // 펫 이동 반경

    public float _currentHp;

    public float PerRadius => _petRadius;

    private void Awake()
    {
        _currentHp = _TowerHp;
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(1.0f, damage - _TowerDefense);
        _currentHp -= finalDamage;

        if (_currentHp <= 0.0f)
        {
            Die();
        }
    }

    public void Attack()
    {

    }

    private void Die()
    {
        // Game Over
    }
}
