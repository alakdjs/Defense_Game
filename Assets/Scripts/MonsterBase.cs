using System;
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

    // Stun
    protected bool _isStunned = false;
    protected float _stunEndTime = 0.0f;
    protected Coroutine _stunCoroutine;

    // 몬스터 사망 이벤트
    public static event Action<MonsterBase> OnAnyMonsterDied;

    public float HpRatio01
    {
        get
        {
            if (_maxHp <= 0.0f)
                return 0.0f;

            return Mathf.Clamp01(_currentHp / _maxHp);
        }
    }

    // FSM
    protected StateMachine _stateMachine;
    protected MonsterIdleState _idleState;
    protected MonsterChaseState _chaseState;
    protected MonsterAttackState _attackState;
    protected MonsterStunState _stunState;
    protected MonsterDeadState _deadState;

    public Transform Target => _target;
    public Animator Animator => _animator;
    public NavMeshAgent Agent => _agent;
    public float AttackRange => _attackRange;

    public bool CanAct => _canAct;

    public MonsterIdleState IdleState => _idleState;
    public MonsterChaseState ChaseState => _chaseState;
    public MonsterAttackState AttackState => _attackState;
    public MonsterStunState StunState => _stunState;
    public MonsterDeadState DeadState => _deadState;

    public StateMachine StateMachine => _stateMachine;

    public void SetCanAct(bool canAct)
    {
        _canAct = canAct;
    }

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
        _stunState = new MonsterStunState(this);
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
            _hpBar = HpBarManager.Instance.GetHpBar(transform, _maxHp, _hpBarWorldOffset, true, true);
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
            _hpBar.ShowHpBar();
            _hpBar.SetHp(_currentHp);
        }

        if (_currentHp <= 0.0f)
        {
            _isDead = true;

            // 기절 코루틴이 돌고 있다면 중단
            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
                _stunCoroutine = null;
            }
            _stateMachine.ChangeState(_deadState);
        }    
    }

    /// <summary>
    /// 타겟 갱신
    /// </summary>
    public virtual void UpdateTarget()
    {
        Transform bestTarget = null;
        float bestDist = float.MaxValue;

        // 1. Tower
        if (_targetTower != null)
        {
            float dist = Vector3.Distance(transform.position, _targetTower.position);
            if (dist < bestDist)
            {
                bestTarget = _targetTower;
                bestDist = dist;
            }
        }

        // 2. Player
        if (_targetPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, _targetPlayer.position);
            if (dist < bestDist)
            {
                bestTarget = _targetPlayer;
                bestDist = dist;
            }
        }

        // 3. Pet (서브타워)
        Collider[] pets = Physics.OverlapSphere(
            transform.position,
            _attackRange + 10.0f,
            LayerMask.GetMask("Pet")
        );

        foreach (Collider pet in pets)
        {
            float dist = Vector3.Distance(transform.position, pet.transform.position);
            if (dist < bestDist)
            {
                bestTarget = pet.transform;
                bestDist = dist;
            }
        }

        _target = bestTarget;
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
        if (_isDead)
            return;

        if (_canAct == false)
            return;

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
    /// 몬스터 기절 상태 (증강)
    /// </summary>
    public void PlayStunAnimation(bool isStun)
    {
        if (_animator == null)
            return;

        if (isStun)
        {
            if (HasAnimatorParameter(_animator, "Stun", AnimatorControllerParameterType.Trigger))
            {
                _animator.SetTrigger("Stun");
            }
        }
    }

    // Animator 파라미터 존재 체크 (없는 파라미터 Set 하면 로그가 지저분해질 수 있어서 방지)
    protected bool HasAnimatorParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        if (animator == null)
            return false;

        foreach (var p in animator.parameters)
        {
            if (p.name == paramName && p.type == type)
                return true;
        }
        return false;
    }

    public void Stun(float duration)
    {
        if (_isDead)
            return;

        if (duration <= 0.0f)
            return;

        float newEndTime = Time.time + duration;

        // 이미 기절 중이면 더 길게 연장
        if (_isStunned)
        {
            _stunEndTime = Mathf.Max(_stunEndTime, newEndTime);
            return;
        }

        _isStunned = true;
        _stunEndTime = newEndTime;

        SetCanAct(false);

        // 공격/추적 중이든 뭐든 기절 상태로 전환
        _stateMachine.ChangeState(_stunState);

        if (_stunCoroutine != null)
            StopCoroutine(_stunCoroutine);

        _stunCoroutine = StartCoroutine(Co_StunRoutine());
    }

    protected System.Collections.IEnumerator Co_StunRoutine()
    {
        // 기절 유지
        while (Time.time < _stunEndTime)
        {
            yield return null;
        }

        // 기절 해제
        _isStunned = false;
        SetCanAct(true);

        // 타겟 갱신 후 상황에 맞게 복귀
        UpdateTarget();

        if (_isDead)
            yield break;

        if (_target != null)
            _stateMachine.ChangeState(_chaseState);
        else
            _stateMachine.ChangeState(_idleState);

        _stunCoroutine = null;
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

        OnAnyMonsterDied?.Invoke(this);
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
