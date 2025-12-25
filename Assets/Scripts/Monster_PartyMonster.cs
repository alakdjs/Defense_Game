using UnityEngine;
using UnityEngine.AI;


public class Monster_PartyMonster : MonsterBase
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _detectRange = 10.0f;
    [SerializeField] private float _attackRange = 2.5f;


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


    protected override void Awake()
    {
        base.Awake();

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
    protected override void Start()
    {
        base.Start();
        _stateMachine.ChangeState(_idleState);
    }

    protected override void UpdateBehavior()
    {
        _stateMachine.Update();
    }

    protected override void Die()
    {
        StateMachine.ChangeState(_deadState);
    }

    // 몬스터 공격 애니메이션
    public void OnAttackAnimationEnd()
    {
        StateMachine.ChangeState(ChaseState);
    }

    // 몬스터 Die 애니메이션 이벤트 종료용 메소드
    public void OnDieAnimationEnd()
    {
        CleanUpHpBar();
        Destroy(gameObject);
    }

}
