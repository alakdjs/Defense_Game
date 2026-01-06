using UnityEngine;
using UnityEngine.AI;

public class PetPatrolState : IState
{
    private PetBase _pet;

    private float _repathInterval = 1.0f; // 순찰 경로 갱신 간격
    private float _repathTimer = 0.0f;

    public PetPatrolState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = false;

        // Walk 애니메이션
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 1.0f);
            _pet.Animator.SetFloat("Vert", 0.5f);
        }

        _repathTimer = 0.0f;
        MoveToRandomPatrolPoint();
    }

    public void Execute()
    {
        if (_pet.Tower == null)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
            return;
        }

        // 몬스터 탐지 -> Chase
        Transform target = _pet.FindNearestMonster();
        if (target != null)
        {
            _pet.SetTargetMonster(target);
            _pet.StateMachine.ChangeState(_pet.ChaseState);
            return;
        }

        // 순찰 목적지 갱신
        _repathTimer += Time.deltaTime;
        if (_repathTimer >= _repathInterval)
        {
            _repathTimer = 0.0f;
            MoveToRandomPatrolPoint();
        }

        // 목적지 도착 후 Idle로 복귀
        if (!_pet.Agent.pathPending && _pet.Agent.remainingDistance <= _pet.Agent.stoppingDistance)
        {
            _pet.StateMachine.ChangeState(_pet.IdleState);
        }
    }

    public void Exit()
    {
        
    }

    // 랜덤 순찰 지점 설정
    private void MoveToRandomPatrolPoint()
    {
        Vector3 center = _pet.Tower.position;
        float radius = _pet.GetPatrolRadius();

        Vector2 random = Random.insideUnitCircle * radius;
        Vector3 targetPos = new Vector3(
            center.x + random.x,
            center.y,
            center.z + random.y
        );

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            _pet.Agent.SetDestination(hit.position);
        }
    }

}
