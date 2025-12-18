using UnityEngine;
using UnityEngine.AI;

public class MonsterNav : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _stopDistance = 1.0f;

    private NavMeshAgent _agent;
    private Animator _anim;
    private bool _isDead = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _target = player.transform;
            }
        }

        if (_agent != null)
        {
            _agent.stoppingDistance = _stopDistance;
        }
    }

    void Update()
    {
        if (_isDead) return;
        if (_agent == null || _target == null) return;

        float dist = Vector3.Distance(transform.position, _target.position);

        if (dist > _stopDistance)
        {
            if (_agent.isStopped)
                _agent.isStopped = false;

            _agent.SetDestination(_target.position);
        }
        else
        {
            if (!_agent.isStopped)
                _agent.isStopped = true;
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

        // Nav 정지
        if (_agent != null)
            _agent.isStopped = true;

        // 콜라이더 비활성화 (중복 충돌 방지)
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // 애니메이션
        if (_anim != null)
            _anim.SetTrigger("Die");

        // 임시 제거
        Destroy(gameObject, 2.0f);
    }
}
