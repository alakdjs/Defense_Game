using UnityEngine;
using UnityEngine.AI;

public class MonsterNav : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _stopDistance = 1.0f; // 플러이어와 멈출 거리

    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _target = player.transform;
            }
            else
            {
                Debug.LogWarning("Player 태그가 없음");
            }
        }

        if (_agent != null)
        {
            _agent.stoppingDistance = _stopDistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent == null || _target == null)
            return;

        float dist = Vector3.Distance(transform.position, _target.position);

        if (dist > _stopDistance)
        {
            if (_agent.isStopped)
            {
                _agent.isStopped = false;
            }

            // NavMesh 경로 계산
            _agent.SetDestination(_target.position);
        }
        else
        {
            if (!_agent.isStopped)
            {
                _agent.isStopped = true;
            }
        }
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
