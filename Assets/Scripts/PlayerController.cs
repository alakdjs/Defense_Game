using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Player Stat")]
    [SerializeField] private float _maxHp = 100.0f; // 체력
    [SerializeField] private float _attack = 1.0f; // 공격력 ( 최종 데미지 = 무기 데미지 x 공격력 )
    [SerializeField] private float _defense = 0.0f; // 방어력
    [SerializeField] private float _speed = 5.0f; // 스피드(이동속도)
    private float _detectRange = 10.0f; // 몬스터 인식 범위
    private float _attackRange = 2.0f; // 무기 공격 범위

    [Header("Auto Attack")]
    [SerializeField] private float _autoAttackInterval = 3.0f;
    [SerializeField] private float _aimRotateSpeed = 10.0f;

    [SerializeField] private AttackRangeUI _attackRangeUI;

    private float _autoAttackTimer = 0.0f;

    [SerializeField] private Animator _animator;

    [Header("Weapon")]
    [SerializeField] private Transform _weaponTarget; // 무기 장착 위치
    [SerializeField] private WeaponType _weaponType = WeaponType.Sword;

    private GameObject _currentWeapon; // 현재 장착된 무기 오브젝트
    private WeaponData _currentWeaponData; // 현재 장착된 무기 데이터
    private FireRifleWeapon _fireRifleWeapon; // Rifle 전용 발사 스크립트

    [SerializeField] private ElementalStatus _elementalStatus;

    private Rigidbody _rb;
    private Camera _mainCam;

    // 이동 타겟
    private Vector3 _targetPosition;
    private bool _hasTarget = false;

    private Vector3 _keyboardInput = Vector3.zero;
    public Vector3 KeyboardInput => _keyboardInput;

    // 증강 시스템용 배율
    private float _maxHpBonus = 0.0f;
    private float _attackBonus = 0.0f;
    private float _defenseBonus = 0.0f;
    private float _speedBonus = 0.0f;
    private float _detectRangeBonus = 0.0f;
    private float _attackRangeBonus = 0.0f;
    private float _autoAttackIntervalBonus = 0.0f;

    // 무기 강화 배율
    private float _weaponDamageBonus = 0.0f;

    // 파동탄 증강 관련
    [SerializeField] private AuraSphereShooter _auraSphereShooter;
    private int _addAuraSphereCount = 0;
    public int AuraSphereCount => _addAuraSphereCount;

    [Header("Sword Slash VFX (Elemental)")]
    [SerializeField] private Transform _slashSpawnPoint;
    [SerializeField] private GameObject _slashFirePrefab;
    [SerializeField] private GameObject _slashElectricPrefab;
    [SerializeField] private GameObject _slashWaterPrefab;
    [SerializeField] private GameObject _slashRockPrefab;
    [SerializeField] private GameObject _slashIcePrefab;
    [SerializeField] private GameObject _slashNormalPrefab;
    [SerializeField] private float _slashLifeTime = 0.35f;
    [SerializeField] private float _slashForwardOffset = 0.6f; // SpawnPoint 없을 때 전방 오프셋


    public float MaxHp => _maxHp + _maxHpBonus;
    public float Attack => _attack + _attackBonus;
    public float Defense => _defense + _defenseBonus;
    public float Speed => _speed + _speedBonus;
    public float DetectRange => _detectRange + _detectRangeBonus;
    public float AttackRange => _attackRange + _attackRangeBonus;

    // 쿨타임 감소 = 기본 + 보너스(보너스는 보통 음수로 들어오게 설계)
    public float AutoAttackInterval => Mathf.Max(0.1f, _autoAttackInterval + _autoAttackIntervalBonus);


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

        if (_elementalStatus == null)
        {
            _elementalStatus = GetComponent<ElementalStatus>();
        }
    }

    void Start()
    {
        // 시작 무기 : Sword (Stick)
        WeaponData startWeapon = WeaponDatabase.Instance.GetWeapon(WeaponType.Sword, WeaponElementType.WoodStick);
        EquipWeapon(startWeapon);

        // 시작 시 무기 상태 Animator 동기화
        SyncWeaponTypeToAnimator();

        _stateMachine.ChangeState(_idleState);
    }

    void Update()
    {
        CheckMouseClick();
        CheckKeyboardInput();
        _stateMachine.Update();
        HandleAutoFire();
        HandleRotationAim();

        // 테스트용: 1번 키 누르면 Rifle 장착
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponData rifle = WeaponDatabase.Instance.GetWeapon(WeaponType.Rifle, WeaponElementType.Normal);

            EquipWeapon(rifle);
        }
        // 테스트용: 2번 키 누르면 Sword 장착
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponData sword = WeaponDatabase.Instance.GetWeapon(WeaponType.Sword, WeaponElementType.Normal);

            EquipWeapon(sword);
        }

        // 테스트용: P 키로 증강 팝업 강제 오픈
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (AugmentPopupController.Instance != null)
            {
                AugmentPopupController.Instance.OpenPopup(2);
            }
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

    // 키보드 wasd 이동 설정
    private void CheckKeyboardInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");  // A, D
        float vertical = Input.GetAxisRaw("Vertical");      // W, S

        if (horizontal != 0 || vertical != 0)
        {
            // 키보드 입력이 있으면 마우스 타겟 취소
            _hasTarget = false;
            _keyboardInput = new Vector3(horizontal, 0, vertical).normalized;

            if (_stateMachine.CurrentState != _moveState)
            {
                _stateMachine.ChangeState(_moveState);
            }
        }
        else
        {
            _keyboardInput = Vector3.zero;
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
        if (data == null || data.WeaponPrefab == null)
        {
            Debug.LogError("[EquipWeapon] WeaponData 또는 Prefab null");
            return;
        }

        // 기존 무기 제거
        if (_currentWeapon != null)
        {
            Destroy(_currentWeapon);
            _fireRifleWeapon = null;
        }

        // 무기 생성
        _currentWeapon = Instantiate(data.WeaponPrefab, _weaponTarget);
        _currentWeapon.transform.localPosition = Vector3.zero;

        // 무기 데이터 및 타입 갱신
        _currentWeaponData = data;
        _weaponType = data.WeaponType;

        _attackRange = data.AttackRange; // 공격 범위 동기화

        // 무기 속성 = 플레이어 속성 (공격/방어 동일 적용)
        if (_elementalStatus != null && _currentWeaponData != null)
        {
            ElementType elem = ElementalCombat.ToElementType(_currentWeaponData.ElementType);
            _elementalStatus.SetElement(elem);
        }

        // UI 반영
        if (_attackRangeUI != null)
        {
            _attackRangeUI.SetRange(AttackRange);

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
        // 몬스터가 있을 때에만 타이머 진행
        _autoAttackTimer += Time.deltaTime;

        if (_autoAttackTimer < AutoAttackInterval)
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

        float range = AttackRange;
        Vector3 center = transform.position + transform.forward * (range * 0.5f);

        // 공격 범위 내 콜라이더 탐색
        Collider[] hits = Physics.OverlapSphere(center, range);

        foreach (var hit in hits)
        {
            MonsterBase monster = hit.GetComponentInParent<MonsterBase>();

            if (monster == null)
                continue;

            float finalDamage = GetFinalDamage();

            ElementType attackerElement = ElementType.Normal;
            if (_elementalStatus != null)
            {
                attackerElement = _elementalStatus.Element;
            }

            DamageInfo dmg = new DamageInfo(finalDamage, attackerElement, gameObject);
            monster.TakeDamage(dmg);
        }

        SpawnSwordHitVFXOnce(hits);
    }

    // 총알 발사 관련 Rifle 애니메이션 이벤트에서 호출
    public void OnRifleFire()
    {
        if (_weaponType != WeaponType.Rifle)
            return;

        if (_fireRifleWeapon != null && _currentWeaponData != null)
        {
            float finalDamage = GetFinalDamage();

            ElementType attackerElement = ElementType.Normal;
            if (_elementalStatus != null)
            {
                attackerElement = _elementalStatus.Element;
            }

            DamageInfo dmg = new DamageInfo(finalDamage, attackerElement, gameObject);
            _fireRifleWeapon.Fire(transform, dmg, AttackRange);
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
        // 키보드 입력 방향으로 회전
        else if (_keyboardInput != Vector3.zero)
        {
            lookDirection = _keyboardInput;
        }

        // 회전 적용
        if (lookDirection.HasValue)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection.Value);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _aimRotateSpeed);
        }

    }

    /// <summary>
    /// 애니메이션 이벤트(베기 시작 프레임)에서 호출: 칼에서 Slash VFX 스폰
    /// </summary>
    public void AnimEvent_SpawnSlashVFX()
    {
        if (_currentWeaponData == null)
            return;

        GameObject prefab = GetSlashPrefab(_currentWeaponData.ElementType);
        if (prefab == null)
            return;

        Vector3 pos;

        if (_slashSpawnPoint != null)
        {
            pos = _slashSpawnPoint.position;
        }
        else
        {
            pos = transform.position + transform.forward * _slashForwardOffset;
            pos.y += 1.0f; 
        }

        Quaternion rot = Quaternion.LookRotation(transform.forward);

        GameObject vfx = Instantiate(prefab, pos, rot);

        // Play On Awake가 꺼져있어도 무조건 보이게 강제 재생
        ParticleSystem[] pss = vfx.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < pss.Length; i++)
        {
            pss[i].Play(true);
        }

        Destroy(vfx, _slashLifeTime);
    }


    /// <summary>
    /// 무기 속성에 맞는 Slash 프리팹 선택
    /// </summary>
    private GameObject GetSlashPrefab(WeaponElementType element)
    {
        switch (element)
        {
            case WeaponElementType.Fire: return _slashFirePrefab;
            case WeaponElementType.Electric: return _slashElectricPrefab;
            case WeaponElementType.Water: return _slashWaterPrefab;
            case WeaponElementType.Rock: return _slashRockPrefab;
            case WeaponElementType.Ice: return _slashIcePrefab;
            default: return _slashNormalPrefab;
        }
    }

    /// <summary>
    /// 칼 공격 1회당 가장 가까운 몬스터 1마리에게만 Hit VFX 스폰
    /// </summary>
    private void SpawnSwordHitVFXOnce(Collider[] hits)
    {
        if (HitVFXManager.Instance == null)
            return;

        MonsterBase closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            MonsterBase monster = hit.GetComponentInParent<MonsterBase>();
            if (monster == null)
                continue;

            float dist = (monster.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = monster;
            }
        }

        if (closest == null)
            return;

        Vector3 hitPos = closest.transform.position;
        hitPos.y += 0.6f; // 몬스터 몸통 높이 보정

        HitVFXManager.Instance.SpawnHitVFX(hitPos, WeaponType.Sword);
    }


    /// <summary>
    /// 데미지 계산 (무기 데미지 + 공격력 + 무기강화보너스)
    /// </summary>
    public float GetFinalDamage()
    {
        if (_currentWeaponData == null)
            return 0;

        return _currentWeaponData.Damage + Attack + _weaponDamageBonus;
    }

    // 증강 시스템 관련 ==============================================================================
    /// <summary>
    /// 스탯 가산 증가
    /// </summary>
    public void AddStatAdditive(StatType statType, float addValue)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                _maxHpBonus += addValue;
                Debug.Log($"[증강] 최대 체력 +{addValue} -> {_maxHpBonus:F2}");
                // PlayerHp UI 갱신
                PlayerHp playerHp = GetComponent<PlayerHp>();
                if (playerHp != null)
                {
                    playerHp.RefreshUI();
                }
                break;

            case StatType.AttackDamage:
                _attackBonus += addValue;
                Debug.Log($"[증강] 공격력 +{addValue} -> {_attackBonus:F2}");
                break;

            case StatType.MoveSpeed:
                _speedBonus += addValue;
                Debug.Log($"[증강] 이동 속도 +{addValue} -> {_speedBonus:F2}");
                break;

            case StatType.Defense:
                _defenseBonus += addValue;
                Debug.Log($"[증강] 방어력 +{addValue} -> {_defenseBonus:F2}");
                break;

            case StatType.AttackSpeed:
                // 공격 속도 = 쿨타임 감소를 "감소량"으로 처리
                // addValue가 0.2라면 쿨타임을 0.2초 줄이는 방식
                _autoAttackIntervalBonus -= addValue;
                Debug.Log($"[증강] 공격 속도(쿨타임 감소) -{addValue} -> {_autoAttackIntervalBonus:F2}");
                break;

            default:
                Debug.LogWarning($"구현되지 않은 StatType: {statType}");
                break;
        }
    }

    /// <summary>
    /// 현재 장착 무기 데미지 증가
    /// </summary>
    public void UpgradeCurrentWeaponDamage(float addValue)
    {
        _weaponDamageBonus += addValue;
        Debug.Log($"[증강] 무기 데미지 +{addValue} -> {_weaponDamageBonus:F2}");
    }

    /// <summary>
    /// 현재 장착 무기 공격 범위 증가
    /// </summary>
    public void IncreaseCurrentWeaponRange(float addValue)
    {
        _attackRangeBonus += addValue;
        _detectRangeBonus += addValue;

        // UI 업데이트
        if (_attackRangeUI != null)
        {
            _attackRangeUI.SetRange(AttackRange);
        }

        Debug.Log($"[증강] 무기 범위 +{addValue} -> {_attackRangeBonus:F2}");
    }


    /// <summary>
    /// 파동탄 증강
    /// </summary>
    /// <param name="add"></param>
    public void AddAuraSphere(int add)
    {
        _addAuraSphereCount += add;
        _addAuraSphereCount = Mathf.Max(0, _addAuraSphereCount);
        Debug.Log($"[증강] 추가 파동탄 +{add} -> {_auraSphereShooter}");
    }

    /// <summary>
    /// 체력 회복 (PlayerHp 컴포넌트로 전달)
    /// </summary>
    public void Heal(float amount)
    {
        PlayerHp playerHp = GetComponent<PlayerHp>();
        if (playerHp != null)
        {
            playerHp.Heal(amount);
            Debug.Log($"[증강] 체력 {amount} 회복");
        }

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

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }

}
