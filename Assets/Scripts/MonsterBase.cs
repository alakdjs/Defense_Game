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

    [Header("HpBar")]
    [SerializeField] protected Vector3 _hpBarWorldOffset = new Vector3(0.0f, 2.0f, 0.0f);
    protected HpBar _hpBar;

    [SerializeField] protected Transform _target;

    protected NavMeshAgent _agent;


    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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
    }

    protected virtual void Update()
    {
        UpdateBehavior();
    }

    /// <summary>
    /// 몬스터별 행동 구현 (FSM / 비FSM 각각 구현)
    /// </summary>
    protected abstract void UpdateBehavior();

    /// <summary>
    /// 데미지 처리 (공통)
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage)
    {
        _currentHp -= damage;

        // 체력바 갱신
        if (_hpBar != null)
        {
            _hpBar.SetHp(_currentHp);
        }

        if (_currentHp <= 0.0f)
        {
            Die();
        }    
    }

    /// <summary>
    /// 몬스터 사망 처리 (자식 클래스에서 구현)
    /// </summary>
    protected abstract void Die();

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
    /// 거리 계산
    /// </summary>
    /// <returns></returns>
    public float DistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;

        return Vector3.Distance(transform.position, _target.position);
    }
}
