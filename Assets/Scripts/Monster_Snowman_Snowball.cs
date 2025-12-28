using UnityEngine;

public class Monster_Snowman_Snowball : MonoBehaviour
{
    [SerializeField] private float _speed = 25.0f;
    [SerializeField] private float _damage = 10.0f;

    private float _traveledDistance;
    private float _maxDistance;
    private bool _isInitialized = false;

    public void Init(float maxDistance)
    {
        _traveledDistance = 0.0f;
        _maxDistance = maxDistance;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        float move = _speed * Time.deltaTime;
        transform.position += transform.forward * move;

        _traveledDistance += move;

        if (_traveledDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어에 맞았을 경우
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

        // 맵 오브젝트에 맞았을 경우
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
