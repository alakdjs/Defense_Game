using UnityEngine;
using UnityEngine.AI;

public class PetIdleState : IState
{
    private PetBase _pet;

    public PetIdleState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = true;

        // Idle 애니메이션
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 0.0f);
            _pet.Animator.SetFloat("Vert", 0.0f);
        }
    }

    public void Execute()
    {
        // 타워 범위 밖으로 나갔으면 타워중앙으로 복귀
        if (_pet.IsOutOfTowerRadius())
        {
            _pet.Agent.isStopped = false;
            _pet.Agent.SetDestination(_pet.Tower.position);

            // 걷기 애니메이션
            if (_pet.Animator != null)
            {
                _pet.Animator.SetFloat("State", 1.0f);
                _pet.Animator.SetFloat("Vert", 1.0f);
            }
            return;
        }

        _pet.UpdateTarget();

        // 타겟이 없으면 Idle 유지
        if (_pet.TargetMonster == null)
            return;

        // 타겟이 있고 공격 가능하면(쿨타임 끝났으면) 바로 Attack
        if (_pet.CanAttack())
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
        // 타겟은 있지만 공격 범위 밖이면 Chase
        else if (!_pet.IsTargetInAttackRange())
        {
            _pet.StateMachine.ChangeState(_pet.ChaseState);
        }
        // 타겟은 있고 범위 안이지만 쿨타임 중이면 Idle 유지
    }

    public void Exit()
    {
        if (_pet.Agent != null)
        {
            _pet.Agent.isStopped = false;
        }
    }
}