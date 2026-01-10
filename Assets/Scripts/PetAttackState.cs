using UnityEngine;
using UnityEngine.AI;

public class PetAttackState : IState
{
    private PetBase _pet;
    private float _dashSpeedMultiplier = 3.0f;
    private float _dashDuration = 1.0f; // 돌진 시간
    private float _dashStartTime;
    private bool _hasDashed;

    public PetAttackState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        // 돌진 시작
        _dashStartTime = Time.time;
        _hasDashed = false;

        // 돌진 속도 설정 (원래 속도에서 배수 적용)
        _pet.Agent.speed *= _dashSpeedMultiplier;
        _pet.Agent.isStopped = false;

        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 1.0f);
            _pet.Animator.SetFloat("Vert", 1.0f);
        }
    }

    public void Execute()
    {
        _pet.UpdateTarget();

        if (_pet.TargetMonster == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        float elapsedTime = Time.time - _dashStartTime;

        // 돌진 중 (1초 동안)
        if (elapsedTime < _dashDuration)
        {
            _pet.Agent.SetDestination(_pet.TargetMonster.position);
        }
        else
        {
            // 돌진이 끝났고, 아직 공격하지 않았다면
            if (!_hasDashed && _pet.IsTargetInAttackRange())
            {
                _pet.PerformAttack();
                _pet.LastAttackTime = Time.time;
                _hasDashed = true;
            }

            // 돌진 완료 후 Idle로 전환
            _pet.StateMachine.ChangeState(_pet.IdleState);
        }
    }

    public void Exit()
    {
        if (_pet.Agent == null)
            return;

        // 원래 속도로 복원
        _pet.Agent.speed /= _dashSpeedMultiplier;
    }
}