using UnityEngine;
using UnityEngine.AI;


public abstract class PetBase : MonoBehaviour, IDamageable
{
    [Header("Base Stat")]
    [SerializeField] protected float _maxHp = 100.0f; // 체력
    [SerializeField] protected float _attackDamage; // 공격력
    [SerializeField] protected float _attackRange = 3.0f;  // 공격 범위
    [SerializeField] protected float _moveSpeed; // 이동 속도

    protected float _currentHp;
    protected bool _isDead = false;

    [Header("HpBar")]
    [SerializeField] protected Vector3 _hpBarWorldOffset = new Vector3(0.0f, 2.0f, 0.0f);
    protected HpBar _hpBar;

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

        // HpBar 풀에서 하나 가져오기(체력바 생성)
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

    // Dead 연출
    private void LateUpdate()
    {
        // 죽었을 때 Melt 효과
        if (_isDead)
        {
            Vector3 scale = transform.localScale;
            scale.y -= Time.deltaTime * 0.5f;
            scale.y = Mathf.Max(0.0f, scale.y);
            transform.localScale = scale;

            if (scale.y <= 0.01f)
            {
                OnDieAnimationEnd();
            }
        }
    }

    // 몬스터 탐지
    public Transform FindNearestMonster()
    {
        TowerMain tower = _tower.GetComponent<TowerMain>();
        if (tower == null)
            return null;

        Collider[] hits = Physics.OverlapSphere(
            _tower.position,
            tower.PetRadius,
            LayerMask.GetMask("Monster")
        );

        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(
                transform.position,
                hit.transform.position
            );

            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    // 현재 공격 대상 몬스터 설정
    public void SetTargetMonster(Transform monster)
    {
        _targetMonster = monster;
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

        if (_hpBar != null)
        {
            _hpBar.SetHp(_currentHp);
        }

        if (_currentHp <= 0.0f)
        {
            _isDead = true;
            Die();
            _stateMachine.ChangeState(_deadState);
        }
    }

    // 공격 처리
    public virtual void PerformAttack()
    {
        ApplyAttackDamage();
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
        if (_hpBar != null && HpBarManager.Instance != null)
        {
            HpBarManager.Instance.ReturnHpbar(_hpBar);
            _hpBar = null;
        }
    }

    public virtual void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }
}
