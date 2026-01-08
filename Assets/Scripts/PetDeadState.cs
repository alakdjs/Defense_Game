using UnityEngine;
using UnityEngine.AI;

public class PetDeadState : IState
{
    private PetBase _pet;

    public PetDeadState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        // NavMeshAgent 정지
        if (_pet.Agent != null)
        {
            _pet.Agent.isStopped = true;
        }
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}
