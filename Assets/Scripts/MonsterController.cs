using UnityEngine;
using UnityEngine.AI;


public class MonsterController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _target;
    [SerializeField] private float _detectRange = 10.0f;
    [SerializeField] private float _attackRange = 1.5f;

    private NavMeshAgent _agent;

    // FSM
    private StateMachine _stateMachine;
    private MonsterIdleState _idleState;
    private MonsterChaseState _chaseState;
    private MonsterAttackState _attackState;
    private MonsterDeadState _deadState;

    public Animator Animator => _animator;
    public Transform Target => _target;
    public NavMeshAgent Agent => _agent;
    public float DetectRange => _detectRange;
    public float AttackRange => _attackRange;

    public MonsterIdleState IdleState => _idleState;
    public MonsterChaseState ChaseState => _chaseState;
    public MonsterAttackState AttackState => _attackState;
    public MonsterDeadState DeadState => _deadState;

    public StateMachine StateMachine => _stateMachine;


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        _stateMachine = new StateMachine();
        _idleState = new MonsterIdleState(this);
        _chaseState = new MonsterChaseState(this);
        _attackState = new MonsterAttackState(this);
        _deadState = new MonsterDeadState(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _target = player.transform;
        }

        _stateMachine.ChangeState(_idleState);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }

    // 거리 계산용
    public float DistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;

        return Vector3.Distance(transform.position, _target.position);
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log("Monster Attack Animation End");
        StateMachine.ChangeState(ChaseState);
    }
}
