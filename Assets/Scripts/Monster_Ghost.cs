using System.Collections;
using UnityEngine;

public class Monster_Ghost : MonsterBase
{
    [SerializeField] float _attackCoolTime = 2.0f;
    private float _lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
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

    public override void Die()
    {
        // Melt 처리, FSM은 Die()처리로 인식
    }

    public override void OnDieAnimationEnd()
    {
        base.OnDieAnimationEnd();
    }

    private void LateUpdate()
    {
        // 죽었을 때만 Melt 효과
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

}
