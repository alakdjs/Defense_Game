using UnityEngine;

public class MonsterIdleState : IState
{
    private MonsterBase _monster;

    public MonsterIdleState(MonsterBase monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        _monster.SetMoveAnimation(false);
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
