using UnityEngine;

public enum WeaponType
{
    Sword = 0,
    Rifle = 1
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Animator _animator;

    [Header("Weapon")]
    [SerializeField] private WeaponType _weaponType = WeaponType.Sword;

    [Header("Bullet Fire Rifle")]
    [SerializeField] private FireRifleWeapon _fireRifleWeapon;

    [SerializeField] private float _autoAttackInterval = 2f;
    private float _autoAttackTimer = 0f;

    [SerializeField] private float _autoAimRange = 8f;
    [SerializeField] private float _aimRotateSpeed = 10f;

    private Rigidbody _rb;
    private Camera _mainCam;

    // 이동 타겟
    private Vector3 _targetPosition;
    private bool _hasTarget = false;

    // FSM
    private StateMachine _stateMachine;
    private PlayerIdleState _idleState;
    private PlayerMoveState _moveState;
    private PlayerDeadState _deadState;

    public PlayerIdleState IdleState => _idleState;
    public PlayerMoveState MoveState => _moveState;
    public PlayerDeadState DeadState => _deadState;

    public StateMachine StateMachine => _stateMachine;
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rb;
    public float MoveSpeed => _moveSpeed;

    public bool HasTarget => _hasTarget;
    public Vector3 TargetPosition => _targetPosition;

    public WeaponType WeaponType
    {
        get => _weaponType;
        set
        {
            _weaponType = value;
            SyncWeaponTypeToAnimator();
        }
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCam = Camera.main;

        if (_animator == null)
            _animator = GetComponent<Animator>();

        // 상태 생성
        _stateMachine = new StateMachine();
        _idleState = new PlayerIdleState(this);
        _moveState = new PlayerMoveState(this);
        _deadState = new PlayerDeadState(this);
    }

    void Start()
    {
        // 시작 시 무기 상태 동기화
        SyncWeaponTypeToAnimator();

        _stateMachine.ChangeState(_idleState);
    }

    void Update()
    {
        CheckMouseClick();
        _stateMachine.Update();
        HandleAutoFire();
        HandleRotationAim();
    }

    // 마우스 클릭 -> 이동 타겟 설정
    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(1))  // 우클릭 이동
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                _targetPosition = hit.point;
                _hasTarget = true;

                if (_stateMachine.CurrentState != _moveState)
                {
                    _stateMachine.ChangeState(_moveState);
                }

            }
        }
    }

    // 타겟 위치 제거(Idle로 복귀할 때 사용)
    public void ClearTarget()
    {
        _hasTarget = false;
    }

    // WeaponType -> Animator 동기화
    public void SyncWeaponTypeToAnimator()
    {
        if (_animator != null)
            _animator.SetInteger("WeaponType", (int)_weaponType);
    }

    private void HandleAutoFire()
    {
        _autoAttackTimer += Time.deltaTime;

        if (_autoAttackTimer < _autoAttackInterval)
            return;

        //Debug.Log($"[AutoAttackTimer] FIRE time={Time.time:F3}");

        _autoAttackTimer = 0f;
        TriggerAttack();
    }

    // 무기 별 공격 트리거
    public void TriggerAttack()
    {
        if (_animator == null) return;

        //Debug.Log($"[TriggerAttack] weapon={_weaponType} time={Time.time:F3}");

        if (_weaponType == WeaponType.Sword)
        {
            _animator.SetTrigger("SwordAttack");
        }
        else if (_weaponType == WeaponType.Rifle)
        {
            _animator.SetTrigger("RifleAttack");
        }
            
    }

    // 총알 발사 관련 Rifle 애니메이션 이벤트
    public void OnRifleFire()
    {
        //Debug.Log($"[OnRifleFire] time={Time.time:F3} frame={Time.frameCount}");

        if (_weaponType != WeaponType.Rifle)
            return;

        if (_fireRifleWeapon != null)
        {
            Debug.Log("총알발사됨");
            _fireRifleWeapon.Fire(transform);
        }
    }

    private Transform FindNearestMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _autoAimRange);

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits) 
        {
            if (!hit.CompareTag("Monster"))
                continue;

            float dist = Vector3.SqrMagnitude(hit.transform.position - transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    // 자동 에임 회전 처리
    private void HandleRotationAim()
    {
        Vector3? lookDirection = null;

        // 몬스터가 있을 때 몬스터 방향
        Transform target = FindNearestMonster();
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                lookDirection = dir;
            }
        }

        // 몬스터가 없을 때 이동 방향
        else if (HasTarget)
        {
            Vector3 dir = TargetPosition - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                lookDirection = dir;
            }
        }

        // 회전 적용
        if (lookDirection.HasValue)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection.Value);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _aimRotateSpeed);
        }

    }

    // 강제로 Dead 상태 전환 가능하도록(테스트용)
    public void CheckDead()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _stateMachine.ChangeState(_deadState);
        }
    }
}
