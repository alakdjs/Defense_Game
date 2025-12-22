using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _fireSpeed = 20.0f;

    private float _traveledDistance;
    private float _maxDistance;
    private bool _isInitialized = false;


    // 발사 시 초기화
    public void Init(float maxDistance)
    {
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
        if (other.CompareTag("Monster"))
        {
            Destroy(other.gameObject, 1.0f); // 몬스터 제거
            Destroy(gameObject); // 총알 제거
            return;
        }

        // 맵 오브젝트에 맞았을 경우
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject); // 총알 제거
        }
    }
}
