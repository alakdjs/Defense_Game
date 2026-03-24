using UnityEngine;

public class MonsterChaseState : IState
{
    private MonsterBase _monster;

    public MonsterChaseState(MonsterBase monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = false;
        _monster.SetMoveAnimation(true);
    }

    public void Execute()
    {
        if (!_monster.CanAct)
            return;

        _monster.UpdateTarget();

        if (_monster.Target == null)
            return;

        float dist = _monster.DistanceToTarget();

        if (dist <= _monster.AttackRange)
        {
            _monster.StateMachine.ChangeState(_monster.AttackState);
            return;
        }

        _monster.Agent.SetDestination(_monster.Target.position);
    }

    public void Exit() 
    {
        _monster.Agent.isStopped = true;
        _monster.SetMoveAnimation(false);
    }
}
