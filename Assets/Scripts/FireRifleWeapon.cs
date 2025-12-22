using UnityEngine;

public class FireRifleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireDelay = 0.3f;

    private float _lastFireTime;

    public void Fire(Transform playerTransform, float attackRange)
    {
        if (Time.time < _lastFireTime + _fireDelay)
            return;

        _lastFireTime = Time.time;

        Vector3 spawnPos = _firePoint.position; // 총구 위치

        Vector3 dir = playerTransform.forward; // 총알 나가는 방향
        dir.y = 0f;
        dir.Normalize();

        GameObject bulletObj = Instantiate(_bulletPrefab, spawnPos, Quaternion.LookRotation(dir));

        bulletObj.transform.SetParent(null);

        // Bullet 초기화
        Bullet bullet = bulletObj.AddComponent<Bullet>();
        if (bullet != null )
        {
            bullet.Init(attackRange);
        }
    }
    
}
