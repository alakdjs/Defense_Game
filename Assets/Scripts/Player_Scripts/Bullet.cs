using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _fireSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * _fireSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터에 맞았을 경우
        if (other.CompareTag("Monster"))
        {
            Destroy(other.gameObject); // 몬스터 제거
            Destroy(gameObject); // 총알 제거
        }
    }
}
