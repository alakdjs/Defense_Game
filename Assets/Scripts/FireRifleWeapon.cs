using UnityEngine;

public class FireRifleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    //[SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireDelay = 0.3f;

    private float _lastFireTime;

    public void Fire(Transform playerTransform, float attackRange)
    {
        //Debug.Log($"[FireRifleWeapon.Fire] time={Time.time:F3}");

        if (Time.time < _lastFireTime + _fireDelay)
            return;

        _lastFireTime = Time.time;

        Vector3 spawnPos =
            playerTransform.position
            + playerTransform.forward * 0.8f
            + Vector3.up * 1.2f;

        Vector3 dir = playerTransform.forward;
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
