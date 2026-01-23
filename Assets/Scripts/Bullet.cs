using UnityEngine;


public class Bullet : MonoBehaviour
{
    [SerializeField] private float _fireSpeed = 20.0f;

    private DamageInfo _damageInfo;
    private float _playerAttackRangeUIDistance;
    private Vector3 _originPos;
    private bool _isInitialized = false;


    // 발사 시 초기화
    public void Init(DamageInfo damageInfo, float playerAttackRangeUIDistance, Vector3 originPos)
    {
        _damageInfo = damageInfo;
        _playerAttackRangeUIDistance = playerAttackRangeUIDistance;
        _originPos = originPos;
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized)
            return;

        float move = _fireSpeed * Time.deltaTime;
        transform.position += transform.forward * move;

        // UI 원: 플레이어 중심, 플레이어 중심에서의 거리 기준으로 총알 파괴
        Vector3 delta = transform.position - _originPos;
        delta.y = 0.0f;
        if (delta.sqrMagnitude >= _playerAttackRangeUIDistance * _playerAttackRangeUIDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 펫에 맞았을 경우 (리턴)
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            return;
        }

        // 몬스터에 맞았을 경우
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && other.GetComponentInParent<MonsterBase>() != null)
        {
            damageable.TakeDamage(_damageInfo);
            Destroy(gameObject);
            return;
        }

        // 맵 오브젝트에 맞았을 경우
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }

    }
}
