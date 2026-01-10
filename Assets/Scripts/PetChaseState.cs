using UnityEngine;

public class PetChaseState : IState
{
    private PetBase _pet;

    public PetChaseState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = false;

        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 1.0f);
            _pet.Animator.SetFloat("Vert", 0.5f);
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

        // 타겟 위치로 이동
        _pet.Agent.SetDestination(_pet.TargetMonster.position);

        // 공격 범위 안에 들어왔고 쿨타임도 끝났으면 Attack
        if (_pet.CanAttack())
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
        // 범위 안에 들어왔지만 쿨타임 중이면 Idle로 (제자리에서 대기)
        else if (_pet.IsTargetInAttackRange())
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
        }
        // 범위 밖이면 계속 Chase
    }

    public void Exit()
    {
    }
}