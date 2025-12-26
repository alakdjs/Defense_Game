using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 모든 몬스터의 공통 베이스
/// </summary>
public abstract class MonsterBase : MonoBehaviour
{
    [Header("Base Stat")]
    [SerializeField] protected float _maxHp = 100.0f;
    protected float _currentHp;
    protected bool _isDead = false;

    [SerializeField] protected float _detectRange = 15.0f;
    [SerializeField] protected float _attackRange = 2.5f;
    [SerializeField] protected float _moveSpeed = 2.5f;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _target;

    [Header("HpBar")]
    [SerializeField] protected Vector3 _hpBarWorldOffset = new Vector3(0.0f, 2.0f, 0.0f);
    protected HpBar _hpBar;

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
    public float DetectRange => _detectRange;
    public float AttackRange => _attackRange;

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

        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _target = player.transform;
            }
        }

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

        _currentHp -= damage;

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
    /// 거리 계산
    /// </summary>
    /// <returns></returns>
    public float DistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;

        return Vector3.Distance(transform.position, _target.position);
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

        PlayerHp playerHp = _target.GetComponent<PlayerHp>();
        if (playerHp != null)
        {
            playerHp.TakeDamage(10.0f);
            Debug.Log("몬스터가 플레이어를 때림");
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
}
