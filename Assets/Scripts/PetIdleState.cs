using UnityEngine;
using UnityEngine.AI;

public class PetIdleState : IState
{
    private PetBase _pet;

    private float _setInterval = 0.3f;   // 상태 판단 주기
    private float _setTimer = 0.0f;

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

        _setTimer = 0.0f;
    }

    public void Execute()
    {
        if (_pet.Tower == null)
            return;

        _setTimer += Time.deltaTime;
        if (_setTimer < _setInterval)
            return;

        _setTimer = 0.0f;

        Transform target = _pet.FindNearestMonster();

        if (target != null)
        {
            _pet.SetTargetMonster(target);
            _pet.StateMachine.ChangeState(_pet.ChaseState);
            return;
        }

        // 몬스터 없으면 순찰
        _pet.StateMachine.ChangeState(_pet.PatrolState);

    }

    public void Exit()
    {
        if (_pet.Agent != null)
        {
            _pet.Agent.isStopped = false;
        }
    }

}
