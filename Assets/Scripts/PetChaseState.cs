using UnityEngine;

public class PetChaseState : IState
{
    private PetBase _pet;

    private float _repathInterval = 0.1f; // 추적 경로 갱신 간격
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

        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 1.0f);
            _pet.Animator.SetFloat("Vert", 0.5f);
        }

        _repathTimer = 0.0f;
    }

    public void Execute()
    {
        if (_pet.TargetMonster == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        Transform target = _pet.FindNearestMonster();

        if (target == null)
        {
            _pet.SetTargetMonster(null);
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        _pet.SetTargetMonster(target);

        // 타워 반경 이탈 시 추적 중단
        if (_pet.IsOutOfTowerRadius())
        {
            _pet.SetTargetMonster(null);
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        // 목적지 갱신
        _repathTimer += Time.deltaTime;

        if (_repathTimer >= _repathInterval)
        {
            _repathTimer = 0.0f;
            _pet.Agent.SetDestination(_pet.TargetMonster.position);
        }

        // 공격 범위에 들어오면 Attack 상태로 전환
        float distance = Vector3.Distance(
            _pet.transform.position,
            _pet.TargetMonster.position
        );

        if (distance <= _pet.AttackRange * 1.2f)
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
    }

    public void Exit()
    {

    }

}
