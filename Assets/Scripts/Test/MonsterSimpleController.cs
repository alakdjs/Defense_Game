using UnityEngine;

public class MonsterSimpleController : MonoBehaviour
{
    public Transform _target;
    public float _moveSpeed = 2f;

    private Rigidbody _rb;
    private Animator _anim;
    private bool _isDead = false;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // 임시: 씬에 배치된 몬스터가 플레이어 타겟으로 따라오게
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트가 없음");
        }
    }

    void FixedUpdate()
    {
        if (_isDead || _target == null) return;

        // 플레이어 쪽으로 이동
        Vector3 dir = (_target.position - transform.position);
        dir.y = 0;

        if (dir.magnitude > 0.1f)
        {
            Vector3 velocity = dir.normalized * _moveSpeed;
            _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);

            transform.forward = dir.normalized;
            _anim.SetBool("Walk", true);
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
            _anim.SetBool("Walk", false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        if (other.CompareTag("Attack"))
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;

        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;

        _anim.SetTrigger("Die");
        Destroy(gameObject, 2.0f);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
