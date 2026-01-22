using UnityEngine;


public class Monster_PartyMonster : MonsterBase
{
    [SerializeField] private float _attackCoolTime = 2.0f;
    private float _lastAttackTime;

    protected override void Awake()
    {
        base.Awake();

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    public override void PerformAttack()
    {
        if (_isDead)
            return;

        if (_canAct == false)
            return;

        // 공격 쿨타임 체크
        if (Time.time < _lastAttackTime + _attackCoolTime)
        {
            StateMachine.ChangeState(IdleState);
            return;
        }

        _lastAttackTime = Time.time;

        base.PerformAttack();
    }

    public void OnAttackHit()
    {
        // 실제 공격 판정
        ApplyAttackDamage();
    }

    // 몬스터 공격 애니메이션
    public void OnAttackAnimationEnd()
    {
        if (_isDead)
            return;

        StateMachine.ChangeState(ChaseState);
    }

    // 몬스터 Die 애니메이션 이벤트 종료용 메소드
    public override void OnDieAnimationEnd()
    {
        base.OnDieAnimationEnd();
    }

}