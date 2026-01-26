using System.Collections;
using UnityEngine;

public class Monster_Ghost : MonsterBase
{
    [SerializeField] float _attackCoolTime = 2.0f;
    private float _lastAttackTime;

    private float _meltDuration = 0.55f;
    private float _meltTime = 0.0f;
    private Vector3 _initialScale;

    protected override void Awake()
    {
        base.Awake();
        _initialScale = transform.localScale;
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
        if (_isDead)
            return;

        if (_canAct == false)
            return;

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
            _meltTime += Time.deltaTime;

            float t = Mathf.Clamp01(_meltTime / _meltDuration);

            float eased = Mathf.Pow(t, 3.0f);

            Vector3 scale = _initialScale;
            scale.y = Mathf.Lerp(_initialScale.y, 0.0f, eased);
            transform.localScale = scale;

            if (t >= 1.0f)
            {
                OnDieAnimationEnd();
            }
        }
    }

}
