using UnityEngine;

public class Monster_Snowman_Snowball : MonoBehaviour
{
    [SerializeField] private float _snowballSpeed = 25.0f;

    private DamageInfo _damageInfo;
    private float _traveledDistance;
    private float _maxDistance;
    private bool _isInitialized = false;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(DamageInfo damageInfo, float maxDistance)
    {
        _damageInfo = damageInfo;
        _traveledDistance = 0.0f;
        _maxDistance = maxDistance;
        _isInitialized = true;

        _rb.linearVelocity = transform.forward * _snowballSpeed;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        _traveledDistance += _snowballSpeed * Time.deltaTime;

        if (_traveledDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
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
