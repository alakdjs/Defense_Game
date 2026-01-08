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

        _pet.Agent.SetDestination(_pet.TargetMonster.position);

        if (_pet.IsTargetInAttackRange())
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
    }

    public void Exit()
    {

    }

}
