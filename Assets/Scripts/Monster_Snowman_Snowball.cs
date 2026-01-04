using UnityEngine;

public class Monster_Snowman_Snowball : MonoBehaviour
{
    [SerializeField] private float _snowballSpeed = 25.0f;

    private float _damage;
    private float _traveledDistance;
    private float _maxDistance;
    private bool _isInitialized = false;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(float damage, float maxDistance)
    {
        _damage = damage;
        _traveledDistance = 0.0f;
        _maxDistance = maxDistance;
        _isInitialized = true;

        _rb.linearVelocity = transform.forward * _snowballSpeed;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        //float move = _snowballSpeed * Time.deltaTime;
        //transform.position += transform.forward * move;
        //_traveledDistance += move;
        _traveledDistance += _snowballSpeed * Time.deltaTime;

        if (_traveledDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            PlayerHp playerHp = other.GetComponent<PlayerHp>();

            if (playerHp != null)
            {
                playerHp.TakeDamage(_damage);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Tower"))
        {
            TowerMain towerHp = other.GetComponent<TowerMain>();

            if (towerHp != null)
            {
                towerHp.TakeDamage(_damage);
            }

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
