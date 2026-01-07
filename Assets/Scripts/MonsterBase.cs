using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 모든 몬스터의 공통 베이스
/// </summary>
public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    [Header("Base Stat")]
    [SerializeField] protected float _maxHp = 100.0f; // 체력
    protected float _currentHp;
    protected bool _isDead = false;
    [SerializeField] protected float _attackDamage = 5.0f; // 공격력
    [SerializeField] protected float _defense = 0.0f; // 방어력
    [SerializeField] protected float _attackRange = 2.5f; // 공격 범위
    [SerializeField] protected float _moveSpeed = 2.5f; // 스피드(이동속도)

    [SerializeField] protected bool _canAct = true;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _target;
    [SerializeField] protected Transform _targetPlayer;
    [SerializeField] protected Transform _targetTower;

    [Header("HpBar")]
    [SerializeField] protected Vector3 _hpBarWorldOffset = new Vector3(0.0f, 2.0f, 0.0f);
    protected HpBar _hpBar;

    [Header("Drop")]
    [SerializeField] protected GameObject _expSpherePrefab;

    protected NavMeshAgent _agent;

    // FSM
    protected StateMachine _stateMachine;
    protected MonsterIdleState _idleState;
    protected MonsterChaseState _chaseState;
    protected MonsterAttackState _attackState;
    protected MonsterDeadState _deadState;

    public Transform Target => _target;
    public Animator Animator => _animator;
    public NavMeshAgent Agent => _agent;
    public float AttackRange => _attackRange;

    public bool CanAct => _canAct;

    public MonsterIdleState IdleState => _idleState;
    public MonsterChaseState ChaseState => _chaseState;
    public MonsterAttackState AttackState => _attackState;
    public MonsterDeadState DeadState => _deadState;

    public StateMachine StateMachine => _stateMachine;


    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        if (_animator == null)
            _animator = GetComponent<Animator>();

        _stateMachine = new StateMachine();
        _idleState = new MonsterIdleState(this);
        _chaseState = new MonsterChaseState(this);
        _attackState = new MonsterAttackState(this);
        _deadState = new MonsterDeadState(this);
    }

    protected virtual void Start()
    {
        _currentHp = _maxHp;

        if (_targetPlayer == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _targetPlayer = player.transform;
            }
        }

        if (_targetTower == null)
        {
            GameObject tower = GameObject.FindGameObjectWithTag("Tower");
            if (tower != null)
            {
                _targetTower = tower.transform;
            }
        }

        UpdateTarget();

        // HpBar 풀에서 하나 가져오기
        if (HpBarManager.Instance != null)
        {
            _hpBar = HpBarManager.Instance.GetHpBar(transform, _maxHp, _hpBarWorldOffset);
            _hpBar.SetHp(_currentHp);
        }

        _stateMachine.ChangeState(_idleState);
    }

    protected virtual void Update()
    {
        if (_isDead)
            return;

        _stateMachine.Update();
    }

    /// <summary>
    /// 공통 데미지 처리
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage)
    {
        if (_isDead)
            return;

        float finalDamage = Mathf.Max(1.0f, damage - _defense);
        _currentHp -= finalDamage;

        // 체력바 갱신
        if (_hpBar != null)
        {
            _hpBar.SetHp(_currentHp);
        }

        if (_currentHp <= 0.0f)
        {
            _isDead = true;
            _stateMachine.ChangeState(_deadState);
        }    
    }

    /// <summary>
    /// 타겟 갱신
    /// </summary>
    public virtual void UpdateTarget()
    {
        if (_targetPlayer == null || _targetTower == null)
        {
            _target = null;
            return;
        }

        if (_targetPlayer == null)
        {
            _target = _targetTower;
            return;
        }

        if (_targetTower == null)
        {
            _target = _targetPlayer;
            return;
        }

        float playerDist = Vector3.Distance(transform.position, _targetPlayer.position);
        float towerDist = Vector3.Distance(transform.position, _targetTower.position);

        if (playerDist < towerDist)
        {
            _target = _targetPlayer;
        }
        else
        {
            _target = _targetTower;
        }
    }

    /// <summary>
    /// 거리 계산
    /// </summary>
    /// <returns></returns>
    public float DistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;

        Collider targetCollider = _target.GetComponent<Collider>();

        if (targetCollider == null)
        {
            // 컬라이더 없으면 transform.position
            return Vector3.Distance(transform.position, _target.position);
        }

        // 타겟 컬라이더 표면 중 가장 가까운 지점
        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        return Vector3.Distance(transform.position, closestPoint);
    }

    /// <summary>
    /// 이동 애니메이션 처리 (FSM에서 호출)
    /// </summary>
    public virtual void SetMoveAnimation(bool isMoving)
    {
        if (_animator == null)
            return;

        _animator.SetBool("IsMoving", isMoving);
    }

    /// <summary>
    /// 공격 처리 (애니메이션 있으면 트리거, 없으면 즉시 공격)
    /// </summary>
    public virtual void PerformAttack()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        else
        {
            // 애니메이션 없는 몬스터 즉시 공격
            ApplyAttackDamage();
            StateMachine.ChangeState(ChaseState);
        }
    }

    /// <summary>
    /// 실제 공격 판정 처리
    /// </summary>
    protected virtual void ApplyAttackDamage()
    {
        if (_target == null)
            return;

        IDamageable damageable = _target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_attackDamage);
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    public virtual void Die()
    {
        if (_isDead == false)
            return;

        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        else
        {
            OnDieAnimationEnd();
        }
    }

    /// <summary>
    /// Die 애니메이션 이벤트 종료 시 호출
    /// </summary>
    public virtual void OnDieAnimationEnd()
    {
        DropExp();
        CleanUpHpBar();
        Destroy(gameObject);
    }

    /// <summary>
    /// HPBar 반환 (사망 시 공통 호출)
    /// </summary>
    protected virtual void CleanUpHpBar()
    {
        if (_hpBar != null && HpBarManager.Instance != null)
        {
            HpBarManager.Instance.ReturnHpbar(_hpBar);
            _hpBar = null;
        }
    }

    /// <summary>
    /// 경험치 드랍
    /// </summary>
    protected virtual void DropExp()
    {
        Instantiate(_expSpherePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }
}
