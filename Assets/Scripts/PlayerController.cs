using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Animator _animator;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponTarget; // 무기 장착 위치
    [SerializeField] private WeaponType _weaponType = WeaponType.Sword;

    [Header("Auto Attack")]
    [SerializeField] private float _autoAttackInterval = 2f;
    [SerializeField] private float _autoAimRange = 8f;
    [SerializeField] private float _aimRotateSpeed = 10f;

    private float _autoAttackTimer = 0f;

    private GameObject _currentWeapon; // 현재 장착된 무기 오브젝트
    private WeaponData _currentWeaponData; // 현재 장착된 무기 데이터
    private FireRifleWeapon _fireRifleWeapon; // Rifle 전용 발사 스크립트

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
        WeaponData startWeapon = WeaponDatabase.Instance.GetDefaultWeapon(WeaponType.Sword);
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
            WeaponData rifle = WeaponDatabase.Instance.GetRandomWeapon(WeaponType.Rifle);

            EquipWeapon(rifle);
        }
    }

    // 마우스 클릭으로 이동 설정
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

    // 무기 장착
    public void EquipWeapon(WeaponData data)
    {
        if (data == null || data.weaponPrefab == null)
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
        _currentWeapon = Instantiate(data.weaponPrefab, _weaponTarget);
        _currentWeapon.transform.localPosition = Vector3.zero;

        // 무기 데이터 및 타입 갱신
        _currentWeaponData = data;
        _weaponType = data.weaponType;

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

    // 총알 발사 관련 Rifle 애니메이션 이벤트에서 호출
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

    // 자동 에임 회전 관련 근처 가까운 몬스터 인식
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

        // 몬스터가 없을 때 이동 방향으로 회전
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
