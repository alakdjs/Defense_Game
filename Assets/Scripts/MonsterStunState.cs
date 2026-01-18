using UnityEngine;

public class MonsterStunState : IState
{
    private MonsterBase _monster;
    
    public MonsterStunState(MonsterBase monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        if (_monster.Agent != null)
        {
            _monster.Agent.isStopped = true;
            _monster.Agent.velocity = Vector3.zero;
        }

        _monster.SetMoveAnimation(false);
        _monster.PlayStunAnimation(true);
    }

    public void Execute()
    {
        // MonsterBase에서 스턴 시간 관리
    }

    public void Exit()
    {
        if (_monster.Agent != null)
        {
            _monster.Agent.isStopped = false;
        }

        _monster.PlayStunAnimation(false);
    }
}
