using UnityEngine;
using UnityEngine.AI;

public class PetIdleState : IState
{
    private PetBase _pet;
    private float _idleStartTime;

    // 복귀 중인지 여부
    private bool _isReturning = false;

    // 복귀 도착 판정 여유값
    private const float _arriveReturn = 1.0f;


    public PetIdleState(PetBase pet)
    {
        _pet = pet;
    }

    public void Enter()
    {
        _idleStartTime = Time.time;
        _isReturning = false;

        if (_pet.Agent == null)
            return;

        _pet.Agent.isStopped = true;

        // Idle 애니메이션
        if (_pet.Animator != null)
        {
            _pet.Animator.SetFloat("State", 0.0f);
            _pet.Animator.SetFloat("Vert", 0.0f);
        }
    }


    public void Execute()
    {
        _pet.UpdateTarget();

        // 복귀 중에는 "도착했는지"만 체크해서 Idle 애니로 돌아오게 처리
        if (_isReturning)
        {
            if (_pet.TargetMonster != null)
            {
                // 복귀 중 몬스터 발견 시 즉시 추적/공격으로 전환
                _isReturning = false;

                if (_pet.CanAttack())
                    _pet.StateMachine.ChangeState(_pet.AttackState);
                else
                    _pet.StateMachine.ChangeState(_pet.ChaseState);

                return;
            }

            if (_pet.Agent != null)
            {
                // 경로 계산 중이 아닐 때 + 목적지 거의 도착했을 때
                if (!_pet.Agent.pathPending && _pet.Agent.remainingDistance <= _pet.Agent.stoppingDistance + _arriveReturn)
                {
                    _isReturning = false;

                    _pet.Agent.isStopped = true;

                    // Idle 복귀
                    if (_pet.Animator != null)
                    {
                        _pet.Animator.SetFloat("State", 0.0f);
                        _pet.Animator.SetFloat("Vert", 0.0f);
                    }

                    return;
                }
            }

            return;
        }

        bool isOut = _pet.IsOutOfTowerRadius();
        float idleElapsed = Time.time - _idleStartTime;

        // Idle 2초 이상 + 반경 밖 + 타겟 없음 => 복귀
        if (isOut && idleElapsed >= 2.0f && _pet.TargetMonster == null)
        {
            if (_pet.Agent == null)
                return;

            _isReturning = true;

            _pet.Agent.isStopped = false;

            // 타워 반경 안쪽으로 복귀
            Vector3 returnPos = _pet.GetReturnPositionToTowerSpawn();
            _pet.Agent.SetDestination(returnPos);

            if (_pet.Animator != null)
            {
                _pet.Animator.SetFloat("State", 1.0f);
                _pet.Animator.SetFloat("Vert", 0.5f);
            }
            return;
        }

        _pet.UpdateTarget();

        // 타겟이 없으면 Idle 유지
        if (_pet.TargetMonster == null)
            return;

        // 타겟이 있고 공격 가능하면(쿨타임 끝났으면) 바로 Attack
        if (_pet.CanAttack())
        {
            _pet.StateMachine.ChangeState(_pet.AttackState);
        }
        // 타겟은 있지만 공격 범위 밖이면 Chase
        else if (!_pet.IsTargetInAttackRange())
        {
            _pet.StateMachine.ChangeState(_pet.ChaseState);
        }
        // 타겟은 있고 범위 안이지만 쿨타임 중이면 Idle 유지
    }

    public void Exit()
    {
        if (_pet.Agent != null)
        {
            _pet.Agent.isStopped = false;
        }
    }
}