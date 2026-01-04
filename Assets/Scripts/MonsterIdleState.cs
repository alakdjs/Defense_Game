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
        // 행동 불가 상태라면 Idle(임시)
        if (!_monster.CanAct)
            return;

        if (_monster.Target != null)
        {
            _monster.StateMachine.ChangeState(_monster.ChaseState);
        }
    }

    public void Exit()
    {

    }
}
