using UnityEngine;

// 플레이어에 붙였음
public class AuraSphereShooter : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private bool _autoFireEnabled = true;
    [SerializeField] private float _autoFireDelay = 3.0f;
    [SerializeField] private float _yOffset = 0.5f; // 플레이어 몸에서 발사 높이
    [SerializeField] private float _auraAttackRange = 10.0f; // 파동탄 사거리

    [SerializeField] private float _damageMultiplier = 0.8f; // 파동탄 데미지 배율(밸런스용)

    [SerializeField] private float _angleOffset = 0.0f; // 패턴 전체 회전 오프셋
    [SerializeField] private bool _rotateOverTime = false; // 시간이 지나면서 패턴이 회전하도록
    [SerializeField] private float _rotateSpeed = 30.0f;   // 초당 회전 각도

    private float _lastAutoFireTime;

    private void Awake()
    {
        if (_player == null)
        {
            _player = GetComponentInParent<PlayerController>();
        }
    }

    private void Update()
    {
        if (!_autoFireEnabled)
            return;

        if (_player == null)
            return;

        // 증강이 없으면 자동 발사도 꺼진 것처럼 동작
        int count = _player.AuraSphereCount;
        if (count <= 0)
            return;

        if (Time.time < _lastAutoFireTime + _autoFireDelay)
            return;

        _lastAutoFireTime = Time.time;

        // 발사 시점에 최신 값 Pull
        float damage = _player.GetFinalDamage() * _damageMultiplier;

        Vector3 originPos = _player.transform.position;

        FireAuraSphereInternal(damage, _auraAttackRange, count, originPos);
    }

    /// <summary>
    /// 플레이어 몸에서 360도 대칭으로 파동탄 발사
    /// 쿨타임 체크는 Update에서
    /// </summary>
    private void FireAuraSphereInternal(float damage, float attackRange, int count, Vector3 originPos)
    {
        // 발사 기준점: 플레이어 위치
        Vector3 pos = transform.position;
        Vector3 spawnPos = pos + Vector3.up * _yOffset;

        // 360도 대칭 발사
        float step = 360.0f / count;

        // 패턴 회전 옵셋
        float offset = _angleOffset;
        if (_rotateOverTime)
        {
            offset += Time.time * _rotateSpeed;
        }

        for (int i = 0; i < count; i++)
        {
            float angle = offset + step * i;

            // 항상 수평면 기준으로 대칭
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            dir.y = 0f;
            dir.Normalize();

            SpawnBullet(spawnPos, dir, damage, attackRange, originPos);
        }
    }

    // 총알 생성
    private void SpawnBullet(Vector3 spawnPos, Vector3 dir, float damage, float attackRange, Vector3 originPos)
    {
        GameObject bulletObj = Instantiate(_bulletPrefab, spawnPos, Quaternion.LookRotation(dir));

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Init(damage, attackRange, originPos);
        }
    }
}
