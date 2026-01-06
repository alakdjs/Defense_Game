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
            _pet.Agent.ResetPath();
        }

        // 이동 애니메이션 정지
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("Speed", 0.0f);
        }

        // 사망 처리 호출 (애니메이션 or 즉시 종료)
        _pet.Die();
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}
