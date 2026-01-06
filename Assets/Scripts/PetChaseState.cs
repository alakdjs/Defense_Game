using UnityEngine;

public class PetChaseState : IState
{
    private PetBase _pet;

    private float _repathInterval = 0.1f; // 경로 갱신 간격
    private float _repathTimer = 0.0f;

    public PetChaseState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = false;
        _pet.SetMoveAnimation(true);
    }

    public void Execute()
    {
        if (_pet.TargetMonster == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        if (_pet.IsOutOfTowerRadius())
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        _repathTimer += Time.deltaTime;

        if (_repathTimer >= _repathInterval)
        {
            _repathTimer = 0.0f;
            _pet.Agent.SetDestination(_pet.TargetMonster.position);
        }

        // Blend Tree용 Speed 값 갱신
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("Speed", 0.5f);
        }

        float distance = Vector3.Distance(
            _pet.transform.position,
            _pet.TargetMonster.position
        );

        // 공격 범위에 들어오면 Attack 상태로 전환
        if (distance <= _pet.AttackRange * 1.2f)
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
    }

    public void Exit()
    {

    }

}
