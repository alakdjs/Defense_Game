using UnityEngine;

public class MonsterDeadState : IState
{
    private MonsterController _monster;

    public MonsterDeadState(MonsterController monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = true;
        _monster.Animator.SetTrigger("Die");
    }
    
    public void Execute()
    {

    }

    public void Exit()
    {

    }
}
