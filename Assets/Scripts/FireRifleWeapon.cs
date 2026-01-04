using UnityEngine;

public class FireRifleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireDelay = 0.3f;

    private float _aimHeightOffset = 0.6f; // 몬스터 키 보정

    private float _lastFireTime;

    public void Fire(Transform playerTransform, float damage, WeaponData weaponData)
    {
        if (Time.time < _lastFireTime + _fireDelay)
            return;

        _lastFireTime = Time.time;

        Vector3 spawnPos = _firePoint.position; // 총구 위치

        // 가장 가까운 몬스터 찾기
        Collider[] hits = Physics.OverlapSphere(
            playerTransform.position,
            weaponData._attackRange
        );

        Transform target = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Monster"))
                continue;

            float dist = Vector3.SqrMagnitude(
                hit.transform.position - playerTransform.position
            );

            if (dist < minDist)
            {
                minDist = dist;
                target = hit.transform;
            }
        }

        Vector3 dir;

        if (target != null)
        {
            // 몬스터 몸통 조준
            Vector3 targetPos = target.position + Vector3.up * _aimHeightOffset;
            dir = (targetPos - spawnPos).normalized;
        }
        else
        {
            // fallback
            dir = playerTransform.forward;
            dir.y = 0f;
            dir.Normalize();
        }

        GameObject bulletObj = Instantiate(_bulletPrefab, spawnPos, Quaternion.LookRotation(dir));

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Init(damage, weaponData._attackRange);
        }
    }
    
}
