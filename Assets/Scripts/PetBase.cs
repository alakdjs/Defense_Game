using UnityEngine;
using UnityEngine.AI;


public abstract class PetBase : MonoBehaviour
{
    [Header("Base Stat")]
    [SerializeField] protected float _maxHp = 100.0f;      // 체력
    [SerializeField] protected float _attackDamage = 5.0f; // 공격력
    [SerializeField] protected float _attackRange = 3.0f;  // 공격 범위
    [SerializeField] protected float _moveSpeed = 2.5f;    // 이동 속도

    protected float _currentHp;
    protected bool _isDead = false;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _tower;
    protected Transform _targetMonster;

    protected NavMeshAgent _agent;

    // FSM
    protected StateMachine _stateMachine;
    protected PetIdleState _idleState;
    protected PetChaseState _chaseState;
    protected PetAttackState _attackState;
    protected PetDeadState _deadState;


    public Transform Tower => _tower;
    public Transform TargetMonster => _targetMonster;
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;

    public float AttackRange => _attackRange;

    public StateMachine StateMachine => _stateMachine;
    public PetIdleState IdleState => _idleState;
    public PetChaseState ChaseState => _chaseState;
    public PetAttackState AttackState => _attackState;
    public PetDeadState DeadState => _deadState;


    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _agent.stoppingDistance = _attackRange;

        if (_animator == null)
            _animator = GetComponent<Animator>();

        _stateMachine = new StateMachine();
        _idleState = new PetIdleState(this);
        _chaseState = new PetChaseState(this);
        _attackState = new PetAttackState(this);
        _deadState = new PetDeadState(this);
    }

    protected virtual void Start()
    {
        _currentHp = _maxHp;

        if (_tower == null)
        {
            GameObject towerObj = GameObject.FindGameObjectWithTag("Tower");
            if (towerObj != null)
            {
                _tower = towerObj.transform;
            }
        }

        _stateMachine.ChangeState(_idleState);
    }

    protected virtual void Update()
    {
        if (_isDead)
            return;

        _stateMachine.Update();
    }

    // 현재 공격 대상 몬스터 설정
    public void SetTargetMonster(Transform monster)
    {
        _targetMonster = monster;
    }

    // 타워 기준 펫 순찰 반경 반환
    public float GetPatrolRadius()
    {
        TowerMain tower = _tower.GetComponent<TowerMain>();
        if (tower != null)
        {
            return tower.PetRadius;
        }

        return 3.0f;
    }

    public bool IsOutOfTowerRadius()
    {
        if (_tower == null)
            return false;

        TowerMain tower = _tower.GetComponent<TowerMain>();
        if (tower == null)
            return false;

        float dist = Vector3.Distance(transform.position, _tower.position);
        return dist > tower.PetRadius;
    }

    // 데미지 처리
    public virtual void TakeDamage(float damage)
    {
        if (_isDead)
            return;

        _currentHp -= damage;

        if (_currentHp <= 0.0f)
        {
            _isDead = true;
            _stateMachine.ChangeState(_deadState);
        }
    }

    // 이동 애니메이션 처리
    public virtual void SetMoveAnimation(bool isMoving)
    {
        if (_animator == null)
            return;

        _animator.SetBool("IsMoving", isMoving);
    }

    // 공격 처리
    public virtual void PerformAttack()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        else
        {
            ApplyAttackDamage();
            _stateMachine.ChangeState(_chaseState);
        }
    }

    // 실제 공격 판정
    protected virtual void ApplyAttackDamage()
    {
        if (_targetMonster == null)
            return;

        IDamageable damageable = _targetMonster.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_attackDamage);
        }
    }

    public virtual void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        else
        {
            OnDieAnimationEnd();
        }
    }

    public virtual void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }
}
