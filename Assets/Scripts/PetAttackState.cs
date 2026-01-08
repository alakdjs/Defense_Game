using UnityEngine;
using UnityEngine.AI;

public class PetAttackState : IState
{
    private PetBase _pet;

    private float _attackDuration = 0.25f; // 돌진 지속 시간
    private float _dashSpeed = 2.5f;
    private float _time = 0.0f;

    private bool _hasAttacked = false;
    private float _originalSpeed;

    public PetAttackState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        _time = 0.0f;
        _hasAttacked = false;

        if (_pet.Agent == null)
            return;

        // 기존 속도 저장
        _originalSpeed = _pet.Agent.speed;

        // 돌진 속도
        _pet.Agent.speed = _originalSpeed * _dashSpeed;
        _pet.Agent.isStopped = false;

        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 1.0f);
            _pet.Animator.SetFloat("Vert", 1.0f);
        }
    }

    public void Execute()
    {
        if (_pet.TargetMonster == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        _time += Time.deltaTime;

        // 타겟 방향으로 돌진
        _pet.Agent.SetDestination(_pet.TargetMonster.position);

        // 공격 판정 (한 번)
        float distance = Vector3.Distance(
            _pet.transform.position,
            _pet.TargetMonster.position
        );

        if (!_hasAttacked && distance <= _pet.AttackRange && Time.time >= _pet.LastAttackTime + _pet.AttackCooltime)
        {
            _hasAttacked = true;
            _pet.PerformAttack(); // 실제 데미지 처리
            _pet.LastAttackTime = Time.time;
        }

        // 돌진 종료
        if (_time >= _attackDuration)
        {
            _pet.StateMachine.ChangeState(_pet.ChaseState);
        }
    }

    public void Exit()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.speed = _originalSpeed;
    }
}
