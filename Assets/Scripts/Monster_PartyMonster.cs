using UnityEngine;


public class Monster_PartyMonster : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

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