using UnityEngine;
using UnityEngine.AI;

public class PetAttackState : IState
{
    private PetBase _pet;
    private float _dashSpeed = 2.5f;
    private float _originalSpeed;

    public PetAttackState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
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
        _pet.UpdateTarget();

        if (_pet.TargetMonster == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        // 돌진 연출
        _pet.Agent.SetDestination(_pet.TargetMonster.position);

        if (_pet.CanAttack())
        {
            _pet.PerformAttack();
            _pet.LastAttackTime = Time.time;
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
