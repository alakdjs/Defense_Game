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

        // Idle 상태이므로 Speed 0 고정
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 0.0f);
            _pet.Animator.SetFloat("Vert", 0.0f);
        }
    }

    public void Execute()
    {
        _pet.UpdateTarget();

        if (_pet.TargetMonster != null)
        {
            _pet.StateMachine.ChangeState(_pet.ChaseState);
        }
    }

    public void Exit()
    {
        if (_pet.Agent != null)
        {
            _pet.Agent.isStopped = false;
        }
    }

}
