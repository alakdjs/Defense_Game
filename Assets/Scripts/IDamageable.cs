using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo damage);

    void TakeDamage(float damage);
}