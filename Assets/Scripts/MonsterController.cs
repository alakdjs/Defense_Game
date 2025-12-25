using UnityEngine;
using UnityEngine.AI;


public class MonsterController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _target;
    [SerializeField] private float _detectRange = 10.0f;
    [SerializeField] private float _attackRange = 2.5f;

    private NavMeshAgent _agent;

    [Header("Stat")]
    [SerializeField] private float _maxHP = 100.0f;
    private float _currentHP;

    [SerializeField] private HpBar _hpBarPrefab;
    [SerializeField] private Vector3 _hpBarWorldOffset = new Vector3(0, 2.0f, 0);
    [SerializeField] private Canvas _uiCanvas;
    private HpBar _hpBarInstance;
    private HpBar _hpBar;


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
        _currentHP = _maxHP;

        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _target = player.transform;
        }

        // HpBar 풀에서 하나 가져오기
        if (HpBarManager.Instance != null)
        {
            _hpBar = HpBarManager.Instance.GetHpBar(transform, _maxHP, _hpBarWorldOffset);
            _hpBar.SetHp(_currentHP);
        }

        _stateMachine.ChangeState(_idleState);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }

    public void TakeDamage(int damage)
    {
        if (StateMachine.CurrentState == DeadState)
            return;

        _currentHP -= damage;

        // 체력바 갱신
        if (_hpBar != null)
        {
            _hpBar.SetHp(_currentHP);
        }

        if (_currentHP <= 0.0f)
        {
            Die();
        }
    }

    // 거리 계산용
    public float DistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;

        return Vector3.Distance(transform.position, _target.position);
    }

    // 몬스터 공격 애니메이션
    public void OnAttackAnimationEnd()
    {
        StateMachine.ChangeState(ChaseState);
    }

    // 몬스터 Die 상태
    public void Die()
    {
        if (StateMachine.CurrentState == DeadState)
            return;

        StateMachine.ChangeState(DeadState);
    }

    // 몬스터 Die 애니메이션 이벤트 종료용 메소드
    public void OnDieAnimationEnd()
    {
        if (_hpBar != null && HpBarManager.Instance != null)
        {
            HpBarManager.Instance.ReturnHpbar(_hpBar);
            _hpBar = null;
        }

        Destroy(gameObject);
    }

}
