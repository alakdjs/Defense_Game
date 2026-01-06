using UnityEngine;
using UnityEngine.AI;

public class PetIdleState : IState
{
    private PetBase _pet;

    private float _searchInterval = 0.3f;   // 몬스터 탐지 주기
    private float _searchTimer = 0.0f;

    private float _patrolInterval = 3.0f;   // 순찰 이동 간격
    private float _patrolTimer = 0.0f;

    public PetIdleState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = true;
        _pet.SetMoveAnimation(false);

        // Idle 상태이므로 Speed 0 고정
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("Speed", 0.0f);
        }

        _searchTimer = 0.0f;
        _patrolTimer = 0.0f;
    }

    public void Execute()
    {
        if (_pet.Tower == null)
            return;

        _searchTimer += Time.deltaTime;
        _patrolTimer += Time.deltaTime;

        // 일정 주기로 몬스터 탐지
        if (_searchTimer >= _searchInterval)
        {
            _searchTimer = 0.0f;

            Transform target = FindNearestMonster();
            if (target != null)
            {
                _pet.SetTargetMonster(target);
                _pet.StateMachine.ChangeState(_pet.ChaseState);
                return;
            }
        }

        // 일정 시간마다 타워 주변을 짧게 순찰
        if (_patrolTimer >= _patrolInterval)
        {
            _patrolTimer = 0.0f;
            MoveToRandomPatrolPoint();
        }
    }

    public void Exit()
    {
        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = false;
    }

    // 몬스터 탐지
    private Transform FindNearestMonster()
    {
        TowerMain tower = _pet.Tower.GetComponent<TowerMain>();
        if (tower == null)
            return null;

        float radius = tower.PetRadius;

        Collider[] hits = Physics.OverlapSphere(
            _pet.transform.position,
            radius,
            LayerMask.GetMask("Monster")
        );

        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(
                _pet.transform.position,
                hit.transform.position
            );

            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    // 타워 반경 내 랜덤 위치로 짧게 이동 (순찰)
    private void MoveToRandomPatrolPoint()
    {
        if (_pet.Agent == null)
            return;

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
            _pet.Agent.isStopped = false;
            _pet.Agent.SetDestination(hit.position);
            _pet.SetMoveAnimation(true);
        }
    }
}
