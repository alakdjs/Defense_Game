using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Player Stat")]
    [SerializeField] private float _maxHp = 100.0f; // 체력
    [SerializeField] private float _attack = 1.0f; // 공격력 ( 최종 데미지 = 무기 데미지 x 공격력 )
    [SerializeField] private float _defense = 0.0f; // 방어력
    [SerializeField] private float _speed = 5.0f; // 스피드(이동속도)
    [SerializeField] private float _detectRange = 6.0f; // 몬스터 인식 범위
    private float _attackRange = 2.0f; // 무기 공격 범위

    [Header("Auto Attack")]
    [SerializeField] private float _autoAttackInterval = 3.0f;
    [SerializeField] private float _aimRotateSpeed = 10.0f;

    [SerializeField] private AttackRangeUI _attackRangeUI;

    private float _autoAttackTimer = 0.0f;
    private bool _hasAutoAttackTarget = false;

    [SerializeField] private Animator _animator;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponTarget; // 무기 장착 위치
    [SerializeField] private WeaponType _weaponType = WeaponType.Sword;

    private GameObject _currentWeapon; // 현재 장착된 무기 오브젝트
    private WeaponData _currentWeaponData; // 현재 장착된 무기 데이터
    private FireRifleWeapon _fireRifleWeapon; // Rifle 전용 발사 스크립트

    private Rigidbody _rb;
    private Camera _mainCam;

    // 이동 타겟
    private Vector3 _targetPosition;
    private bool _hasTarget = false;

    public float MaxHp => _maxHp;
    public float Attack => _attack;
    public float Defense => _defense;
    public float Speed => _speed;
    public float DetectRange => _detectRange;
    public float AttackRange => _attackRange;


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

    public bool HasTarget => _hasTarget;
    public Vector3 TargetPosition => _targetPosition;
    public WeaponType WeaponType => _weaponType;


    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCam = Camera.main;

        if (_animator == null)
            _animator = GetComponent<Animator>();

        // FSM 상태 생성
        _stateMachine = new StateMachine();
        _idleState = new PlayerIdleState(this);
        _moveState = new PlayerMoveState(this);
        _deadState = new PlayerDeadState(this);
    }

    void Start()
    {
        // 시작 무기 : Sword (Stick)
        WeaponData startWeapon = WeaponDatabase._Instance.GetDefaultWeapon(WeaponType.Sword);
        EquipWeapon(startWeapon);

        // 시작 시 무기 상태 Animator 동기화
        SyncWeaponTypeToAnimator();

        _stateMachine.ChangeState(_idleState);
    }

    void Update()
    {
        CheckMouseClick();
        _stateMachine.Update();
        HandleAutoFire();
        HandleRotationAim();

        // 테스트용: 1번 키 누르면 Rifle 장착
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponData rifle = WeaponDatabase._Instance.GetRandomWeapon(WeaponType.Rifle);

            EquipWeapon(rifle);
        }
    }

    private void FixedUpdate()
    {
        // 물리 충돌로 생긴 회전 속도 제거
        if (_rb != null)
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = 0f; // 항상 지면에 고정
        transform.position = pos;

    }

    // 마우스 클릭으로 이동 설정
    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(1))  // 우클릭 이동
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f))
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
        _targetPosition = transform.position;
    }

    // 무기 장착
    public void EquipWeapon(WeaponData data)
    {
        if (data == null || data._weaponPrefab == null)
        {
            Debug.LogError("[EquipWeapon] WeaponData 또는 Prefab null");
            return;
        }

        // 같은 무기 데이터면 교체하지 않음
        if (_currentWeaponData == data)
            return;

        // 기존 무기 제거
        if (_currentWeapon != null)
        {
            Destroy(_currentWeapon);
            _fireRifleWeapon = null;
        }

        // 무기 생성
        _currentWeapon = Instantiate(data._weaponPrefab, _weaponTarget);
        _currentWeapon.transform.localPosition = Vector3.zero;

        // 무기 데이터 및 타입 갱신
        _currentWeaponData = data;
        _weaponType = data._weaponType;

        _attackRange = data._attackRange; // 공격 범위 동기화

        // UI 반영
        if (_attackRangeUI != null)
        {
            _attackRangeUI.SetRange(_attackRange);

        }

        // Rifle일 경우 발사 스크립트 캐싱
        _fireRifleWeapon = _currentWeapon.GetComponent<FireRifleWeapon>();

        SyncWeaponTypeToAnimator();
    }

    // WeaponType -> Animator 동기화
    public void SyncWeaponTypeToAnimator()
    {
        if (_animator != null)
            _animator.SetInteger("WeaponType", (int)_weaponType);
    }

    // 자동 공격
    private void HandleAutoFire()
    {
        // 주변에 몬스터가 없으면
        Transform target = FindNearestMonster();
        if (target == null)
        {
            // 타이머 멈추거나 초기화
            _autoAttackTimer = 0.0f;
            _hasAutoAttackTarget = false;
            return;
        }

        // 몬스터를 처음 발견했을 때
        if (!_hasAutoAttackTarget)
        {
            _hasAutoAttackTarget = true;
            _autoAttackTimer = _autoAttackInterval; // 즉시 공격
        }

        // 몬스터가 있을 때에만 타이머 진행
        _autoAttackTimer += Time.deltaTime;

        if (_autoAttackTimer < _autoAttackInterval)
            return;

        _autoAttackTimer = 0f;
        TriggerAttack();
    }

    // 무기 별 공격 트리거
    public void TriggerAttack()
    {
        if (_animator == null) return;

        if (_weaponType == WeaponType.Sword)
        {
            _animator.SetTrigger("SwordAttack");
        }
        else if (_weaponType == WeaponType.Rifle)
        {
            _animator.SetTrigger("RifleAttack");
        }
            
    }

    // Sword 공격 판정 (애니메이션 이벤트에서 호출)
    public void OnSwordHit()
    {
        if (_currentWeaponData == null)
            return;

        Vector3 center = transform.position + transform.forward * (_currentWeaponData._attackRange * 0.5f);

        // 공격 범위 내 콜라이더 탐색
        Collider[] hits = Physics.OverlapSphere(center, _currentWeaponData._attackRange);

        foreach (var hit in hits)
        {
            if (!hit.transform.root.CompareTag("Monster"))
                continue;

            MonsterBase monster = hit.transform.root.GetComponent<MonsterBase>();

            if (monster != null)
            {
                monster.TakeDamage(GetFinalDamage());
            }

        }
    }

    // 총알 발사 관련 Rifle 애니메이션 이벤트에서 호출
    public void OnRifleFire()
    {
        if (_weaponType != WeaponType.Rifle)
            return;

        if (_fireRifleWeapon != null && _currentWeaponData != null)
        {
            float finalDamage = GetFinalDamage();
            _fireRifleWeapon.Fire(transform, finalDamage, _currentWeaponData);
        }
    }

    // 자동 에임 회전 관련 근처 가까운 몬스터 인식
    private Transform FindNearestMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRange);

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

        // 몬스터가 있을 때 몬스터 방향으로 회전
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

        // 몬스터가 없고, 이동 중일 때만 이동 방향으로 회전
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

    /// <summary>
    /// 데미지 계산
    /// </summary>
    public float GetFinalDamage()
    {
        if (_currentWeaponData == null)
            return 0;

        return _currentWeaponData._damage * _attack;
    }

    /// <summary>
    /// 스탯 증가 관련 (레벨업), 회복은 PlayerHp.cs에서 
    /// </summary>
    public void AddMaxHp(float value)
    {
        _maxHp += value;
    }

    public void AddAttack(float value)
    {
        _attack += value;
    }

    public void AddDefense(float value)
    {
        _defense += value;
    }

    public void OnDeadAnimationEnd()
    {
        Destroy(gameObject);
    }

}
