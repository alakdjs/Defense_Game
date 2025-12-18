using UnityEngine;

public class FireRifleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireDelay = 0.3f;

    private float _lastFireTime;

    public void Fire(Transform playerTransform)
    {
        //Debug.Log($"[FireRifleWeapon.Fire] time={Time.time:F3}");

        if (Time.time < _lastFireTime + _fireDelay)
            return;

        _lastFireTime = Time.time;

        Vector3 dir = playerTransform.forward;
        dir.y = 0f;
        dir.Normalize();

        Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(dir));
    }
    
}
