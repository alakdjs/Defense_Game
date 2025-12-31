using UnityEngine;


public class Bullet : MonoBehaviour
{
    [SerializeField] private float _fireSpeed = 20.0f;

    private float _damage;
    private float _traveledDistance;
    private float _maxDistance;
    private bool _isInitialized = false;


    // 발사 시 초기화
    public void Init(float damage, float maxDistance)
    {
        _damage = damage;
        _traveledDistance = 0.0f;
        _maxDistance = maxDistance;
        _isInitialized = true;
    }


    void Update()
    {
        if (!_isInitialized)
            return;

        float move = _fireSpeed * Time.deltaTime;
        transform.position += transform.forward * move;

        _traveledDistance += move;

        if (_traveledDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터에 맞았을 경우
        MonsterBase monster = other.GetComponentInParent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeDamage(_damage);
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
