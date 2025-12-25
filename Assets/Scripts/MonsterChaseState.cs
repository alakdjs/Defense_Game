using UnityEngine;

public class MonsterChaseState : IState
{
    private Monster_PartyMonster _monster;

    public MonsterChaseState(Monster_PartyMonster monster)
    {
        _monster = monster;
    }

    public void Enter()
    {
        _monster.Agent.isStopped = false;
        _monster.Animator.SetBool("IsMoving", true);
    }

    public void Execute()
    {
        if (_monster.Target == null)
            return;

        _monster.Agent.SetDestination(_monster.Target.position);

        if (_monster.DistanceToTarget() <= _monster.AttackRange)
        {
            _monster.StateMachine.ChangeState(_monster.AttackState);
        }
    }

    public void Exit() 
    {
        _monster.Agent.isStopped = true;
        _monster.Animator.SetBool("IsMoving", false);
    }
}
