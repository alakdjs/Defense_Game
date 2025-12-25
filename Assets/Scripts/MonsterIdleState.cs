using UnityEngine;

public class MonsterIdleState : IState
{
    private Monster_PartyMonster _monster;

    public MonsterIdleState(Monster_PartyMonster monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        _monster.Animator.SetBool("IsMoving", false);
    }

    public void Execute()
    {
        // 플레이어 감지
        if (_monster.DistanceToTarget() <= _monster.DetectRange)
        {
            _monster.StateMachine.ChangeState(_monster.ChaseState);
        }
    }

    public void Exit()
    {

    }
}
