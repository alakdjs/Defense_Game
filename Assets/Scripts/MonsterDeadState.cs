using UnityEngine;

public class MonsterDeadState : IState
{
    private MonsterBase _monster;

    public MonsterDeadState(MonsterBase monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;

        _monster.Die();
    }
    
    public void Execute()
    {

    }

    public void Exit()
    {

    }
}
